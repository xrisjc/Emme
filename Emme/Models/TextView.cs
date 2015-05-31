using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emme.Models
{
  class TextView : IEnumerable<string>
  {
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
      gapBuffer.Insert(CaretIndex(currentLine, caretPosition), value);
      Shift(currentLine, 1);
      caretPosition = caretPosition.MoveColumn(Direction.Next);
    }

    public void InsertNewLine()
    {
      int splitIndex = CaretIndex(currentLine, caretPosition);
      var splitSpans = CurrentLineSpan.Split(splitIndex);

      CurrentLineSpan = splitSpans.Item1;
      lines.AddAfter(currentLine, splitSpans.Item2);

      currentLine = currentLine.Next;
      caretPosition = caretPosition.MoveLine(Direction.Next).SetColumn(0);
    }

    public void Delete()
    {
      if (caretPosition.Column < Length(currentLine))
      {
        gapBuffer.Delete(CaretIndex(currentLine, caretPosition));
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
      if (caretPosition != Position.BufferStart)
      {
        MoveToPrevious();
        Delete();
      }
    }

    public void MoveToPrevious()
    {
      if (caretPosition.Column > 0)
      {
        caretPosition = caretPosition.MoveColumn(Direction.Previous);
      }
      else if (IsLineBefore(currentLine))
      {
        currentLine = currentLine.Previous;
        caretPosition = caretPosition.MoveLine(Direction.Previous)
                                     .SetColumn(Length(currentLine));
      }
    }

    /// <summary>
    /// Moves current position to next character.
    /// </summary>
    /// <returns>Updated position after move.</returns>
    public void MoveToNext()
    {
      if (caretPosition.Column < Length(currentLine))
      {
        caretPosition = caretPosition.MoveColumn(Direction.Next);
      }
      else if (IsLineAfter(currentLine))
      {
        currentLine = currentLine.Next;
        caretPosition = caretPosition.MoveLine(Direction.Next).SetColumn(0);
      }
    }

    public void MoveToPreviousLine()
    {
      if (IsLineBefore(currentLine))
      {
        currentLine = currentLine.Previous;
        caretPosition = caretPosition.MoveLine(Direction.Previous)
                                     .ClampColumn(Length(currentLine));
      }
    }

    public void MoveToNextLine()
    {
      if (IsLineAfter(currentLine))
      {
        currentLine = currentLine.Next;
        caretPosition = caretPosition.MoveLine(Direction.Next)
                                     .ClampColumn(Length(currentLine));
      }
    }

    public void MoveToLineStart()
    {
      caretPosition = caretPosition.SetColumn(0);
    }

    public void MoveToLineEnd()
    {
      caretPosition = caretPosition.SetColumn(Length(currentLine));
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

            case '\n':
              sb.Append('↵');
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
        yield return sb.ToString();
      }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return GetEnumerator();
    }
  }
}
