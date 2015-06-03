//
// File: Win32.cs
//
// Copyright (C) 2015  Christopher Cowan
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//

using System;
using System.Runtime.InteropServices;

namespace Emme.UI
{
  /// <summary>
  /// Win32 API interop functions.
  /// </summary>
  static class Win32
  {
    /// <summary>
    /// Bitmap handle to create a solid color caret.
    /// </summary>
    public const int SOLID_BITMAP_HANDLE = 0;

    /// <summary>
    /// Win32 function to create a caret.
    /// </summary>
    /// <param name="hWnd">Handle to the window to create the caret in</param>
    /// <param name="hBitmap">Handle to the caret bitmap. 0 (NULL) means solid bitmap.</param>
    /// <param name="nWidth">Height in logical units.</param>
    /// <param name="nHeight">Width in logical units.</param>
    /// <returns>True if succeeded, false otherwise.</returns>
    [DllImport("User32.dll")]
    public static extern bool CreateCaret(IntPtr hWnd, int hBitmap, int nWidth, int nHeight);

    /// <summary>
    /// Win32 function to set the caret's position.
    /// </summary>
    /// <param name="x">Caret's x-coordinate</param>
    /// <param name="y">Caret's y-coordinate</param>
    /// <returns>True if succeeded, false otherwise.</returns>
    [DllImport("User32.dll")]
    public static extern bool SetCaretPos(int x, int y);

    /// <summary>
    /// Win32 function to destroy caret's shape and remove it from screen.
    /// </summary>
    /// <returns>True if succeeded, false otherwise.</returns>
    [DllImport("User32.dll")]
    public static extern bool DestroyCaret();

    /// <summary>
    /// Win32 function to make caret visible.
    /// </summary>
    /// <param name="hWnd">Handle to window caret is in.</param>
    /// <returns>True if succeeded, false otherwise.</returns>
    [DllImport("User32.dll")]
    public static extern bool ShowCaret(IntPtr hWnd);

    /// <summary>
    /// Win32 function to remove caret from screen.
    /// </summary>
    /// <param name="hWnd">Handle to window caret is in.</param>
    /// <returns>True if succeeded, false otherwise.</returns>
    [DllImport("User32.dll")]
    public static extern bool HideCaret(IntPtr hWnd);
  }
}