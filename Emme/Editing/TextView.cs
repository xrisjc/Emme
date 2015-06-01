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

    readonly GapBuffer gapBuffer;
    readonly LinkedList<Span> lines = new LinkedList<Span>();
    LinkedListNode<Span> currentLine;
    Position caretPosition;

    public TextView(GapBuffer gapBuffer)
    {
      this.gapBuffer = gapBuffer;
      caretPosition = Position.BufferStart;
      lines.AddFirst(new Span(0, 0));
      currentLine = lines.First;
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

    Span CurrentLineSpan
    {
      get { return currentLine.Value; }
      set { currentLine.Value = value; }
    }

    static void Shift(LinkedListNode<Span> line, int delta)
    {
      line.Value = line.Value.MoveEnd(delta);
      while ((line = line.Next) != null)
      {
        line.Value = line.Value.Move(delta);
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

    static int CaretIndex(LinkedListNode<Span> line, Position caretPosition)
    {
      return line.Value.Start + caretPosition.Column;
    }

    static int Length(LinkedListNode<Span> line)
    {
      return line.Value.Length;
    }

    public void Insert(char value)
    {
      gapBuffer.Insert(CaretIndex(currentLine, CaretPosition), value);
      Shift(currentLine, 1);
      CaretPosition = CaretPosition.MoveColumn(Direction.Next);
    }

    public void InsertNewLine()
    {
      int splitIndex = CaretIndex(currentLine, CaretPosition);
      var splitSpans = CurrentLineSpan.Split(splitIndex);

      CurrentLineSpan = splitSpans.Item1;
      lines.AddAfter(currentLine, splitSpans.Item2);

      currentLine = currentLine.Next;
      CaretPosition = CaretPosition.MoveLine(Direction.Next).SetColumn(0);
    }

    public void Delete()
    {
      if (CaretPosition.Column < Length(currentLine))
      {
        gapBuffer.Delete(CaretIndex(currentLine, CaretPosition));
        Shift(currentLine, -1);
      }
      else if (IsLineAfter(currentLine))
      {
        CurrentLineSpan = CurrentLineSpan.Join(currentLine.Next.Value);
        lines.Remove(currentLine.Next);
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
      else if (IsLineBefore(currentLine))
      {
        currentLine = currentLine.Previous;
        CaretPosition = CaretPosition.MoveLine(Direction.Previous)
                                     .SetColumn(Length(currentLine));
      }
    }

    /// <summary>
    /// Moves current position to next character.
    /// </summary>
    /// <returns>Updated position after move.</returns>
    public void MoveToNext()
    {
      if (CaretPosition.Column < Length(currentLine))
      {
        CaretPosition = CaretPosition.MoveColumn(Direction.Next);
      }
      else if (IsLineAfter(currentLine))
      {
        currentLine = currentLine.Next;
        CaretPosition = CaretPosition.MoveLine(Direction.Next).SetColumn(0);
      }
    }

    public void MoveToPreviousLine()
    {
      if (IsLineBefore(currentLine))
      {
        currentLine = currentLine.Previous;
        CaretPosition = CaretPosition.MoveLine(Direction.Previous)
                                     .ClampColumn(Length(currentLine));
      }
    }

    public void MoveToNextLine()
    {
      if (IsLineAfter(currentLine))
      {
        currentLine = currentLine.Next;
        CaretPosition = CaretPosition.MoveLine(Direction.Next)
                                     .ClampColumn(Length(currentLine));
      }
    }

    public void MoveToLineStart()
    {
      CaretPosition = CaretPosition.SetColumn(0);
    }

    public void MoveToLineEnd()
    {
      CaretPosition = CaretPosition.SetColumn(Length(currentLine));
    }

    public IEnumerator<string> GetEnumerator()
    {
      StringBuilder sb = new StringBuilder();
      for (var line = lines.First; line != null; line = line.Next)
      {
        sb.Clear();
        for (int i = line.Value.Start; i < line.Value.End; i++)
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
        if (line.Next == null)
        {
          sb.Append('¤');
        }
        else
        {
          sb.Append('↵');
        }
        yield return sb.ToString();
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
