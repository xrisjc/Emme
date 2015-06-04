using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Emme.Models;

namespace Emme.Editing
{
  class TextView : IEnumerable<string>
  {
    /// <summary>
    /// Event fired when the caret's position changes.
    /// </summary>
    public event EventHandler<PositionEventArgs> CaretPositionChanged;

    readonly GapBuffer<char> gapBuffer;
    readonly GapBuffer<Span> lines = new GapBuffer<Span>();
    Position caretPosition = Position.BufferStart;

    public TextView(GapBuffer<char> gapBuffer)
    {
      this.gapBuffer = gapBuffer;
      lines.Insert(0, new Span(0, 0));
    }

    public Position CaretPosition
    {
      get { return caretPosition; }
      private set
      {
        bool caretPositionDidChange = (caretPosition != value);
        caretPosition = value;
        if (caretPositionDidChange && CaretPositionChanged != null)
        {
          CaretPositionChanged(this, new PositionEventArgs(caretPosition));
        }
      }
    }

    private void Shift(int delta)
    {
      lines[CaretPosition.Line] = lines[CaretPosition.Line].MoveEnd(delta);
      for (int i = CaretPosition.Line + 1; i < lines.Count; i++)
      {
        lines[i] = lines[i].Move(delta);
      }
    }

    static bool IsLineBefore(LinkedListNode<Span> line)
    {
      return line.Previous != null;
    }

    static bool IsLineAfter(LinkedListNode<Span> line)
    {
      return line.Next != null;
    }

    static int CaretIndex(Span line, Position caretPosition)
    {
      return line.Start + caretPosition.Column;
    }

    static int Length(LinkedListNode<Span> line)
    {
      return line.Value.Length;
    }

    public void Insert(char value)
    {
      int insertIndex = CaretIndex(lines[CaretPosition.Line], CaretPosition);
      gapBuffer.Insert(insertIndex, value);
      Shift(1);
      CaretPosition = CaretPosition.MoveColumn(Direction.Next);
    }

    public void InsertNewLine()
    {
      int splitIndex = CaretIndex(lines[CaretPosition.Line], CaretPosition);
      Tuple<Span, Span> splitSpans = lines[CaretPosition.Line].Split(splitIndex);
      lines[CaretPosition.Line] = splitSpans.Item1;
      CaretPosition = CaretPosition.MoveLine(Direction.Next).SetColumn(0);
      lines.Insert(CaretPosition.Line, splitSpans.Item2);
    }

    public void Delete()
    {
      if (CaretPosition.Column < lines[CaretPosition.Line].Length)
      {
        int deleteIndex = CaretIndex(lines[CaretPosition.Line], CaretPosition);
        gapBuffer.Delete(deleteIndex);
        Shift(-1);
      }
      else if (CaretPosition.Line < lines.Count - 1)
      {
        lines[CaretPosition.Line] = lines[CaretPosition.Line].Join(lines[CaretPosition.Line + 1]);
        lines.Delete(CaretPosition.Line + 1);
      }
    }

    public void DeletePrevious()
    {
      if (CaretPosition != Position.BufferStart)
      {
        MoveToPrevious();
        Delete();
      }
    }

    public void MoveToPrevious()
    {
      if (CaretPosition.Column > 0)
      {
        CaretPosition = CaretPosition.MoveColumn(Direction.Previous);
      }
      else if (CaretPosition.Line > 0)
      {
        CaretPosition = CaretPosition.MoveLine(Direction.Previous)
                                     .SetColumn(lines[CaretPosition.Line - 1].Length);
      }
    }

    /// <summary>
    /// Moves current position to next character.
    /// </summary>
    /// <returns>Updated position after move.</returns>
    public void MoveToNext()
    {
      if (CaretPosition.Column < lines[CaretPosition.Line].Length)
      {
        CaretPosition = CaretPosition.MoveColumn(Direction.Next);
      }
      else if (CaretPosition.Line < lines.Count - 1)
      {
        CaretPosition = CaretPosition.MoveLine(Direction.Next).SetColumn(0);
      }
    }

    public void MoveToPreviousLine()
    {
      if (CaretPosition.Line > 0)
      {
        CaretPosition = CaretPosition.MoveLine(Direction.Previous)
                                     .ClampColumn(lines[CaretPosition.Line - 1].Length);
      }
    }

    public void MoveToNextLine()
    {
      if (CaretPosition.Line < lines.Count - 1)
      {
        CaretPosition = CaretPosition.MoveLine(Direction.Next)
                                     .ClampColumn(lines[CaretPosition.Line + 1].Length);
      }
    }

    public void MoveToLineStart()
    {
      CaretPosition = CaretPosition.SetColumn(0);
    }

    public void MoveToLineEnd()
    {
      CaretPosition = CaretPosition.SetColumn(lines[CaretPosition.Line].Length);
    }

    public IEnumerator<string> GetEnumerator()
    {
      StringBuilder sb = new StringBuilder();
      for (var line = 0; line < lines.Count; line++)
      {
        sb.Clear();
        for (int i = lines[line].Start; i < lines[line].End; i++)
        {
          switch (gapBuffer[i])
          {
            case ' ':
              sb.Append('·');
              break;

            default:
              sb.Append(gapBuffer[i]);
              break;
          }

        }
        sb.Append(line + 1 == lines.Count ? '¤' : '↵');
        yield return sb.ToString();
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
