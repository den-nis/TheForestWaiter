using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheForestWaiter.Debugging.DebugConsole
{
    class TableBuilder
    {
        private const char HORIZONTAL_BORDER = '-';
        private const char VERTICAL_BORDER = '|';
        private const char CORNER = 'o';
        private const int CELL_MARGIN = 3;

        private int _columns = -1;
        private readonly List<object[]> _rows = new();
        private readonly bool _header;

        public TableBuilder(bool header = false)
        {
            _header = header;
        }

        public TableBuilder WriteRow(params object[] row)
        {
            _columns =
                (_columns == row.Length || _columns == -1) ?
                row.Length :
                throw new InvalidOperationException($"Invalid amount of columns expected ({_columns}) got ({_rows.Count})");

            _rows.Add(row);
            return this;
        }

        public string[,] To2DArray()
        {
            string[,] table = new string[_columns, _rows.Count];

            for (int y = 0; y < _rows.Count; y++)
            {
                for (int x = 0; x < _columns; x++)
                {
                    table[x, y] = _rows[y][x]?.ToString();
                }
            }

            return table;
        }

        public override string ToString()
        { 
            var table = To2DArray();
            int[] columnSizes = Enumerable.Range(0, table.GetLength(0))
                .Select(i => GetColumnSize(table, i) + CELL_MARGIN)
                .ToArray();

            var bar = $"{CORNER}{new string(HORIZONTAL_BORDER, columnSizes.Sum() + columnSizes.Length - 1)}{CORNER}";

            var sb = new StringBuilder();
            sb.AppendLine(bar);
            for (int y = 0; y < table.GetLength(1); y++)
            {
                if (y == 1 && _header)
                    sb.AppendLine(bar);

                for (int x = 0; x < table.GetLength(0); x++)
                {
                    string cell = ClipStr(table[x, y], columnSizes[x], ' ');
                    sb.Append($"{VERTICAL_BORDER}{cell}");
                }
                sb.Append($"{VERTICAL_BORDER}{Environment.NewLine}");
            }
            sb.AppendLine(bar);

            return sb.ToString();
        }

        private static int GetColumnSize(string[,] table, int index)
        {
            int result = 0;
            for (int i = 0; i < table.GetLength(1); i++)
            {
                result = Math.Max(table[index, i]?.Length ?? 0, result);
            }
            return result;
        }


        private static string ClipStr(string value, int length, char pad)
        {
            value ??= string.Empty;

            if (value.Length > length)
                return value.Substring(0, length);

            if (value.Length < length)
                return value + new string(pad, length - value.Length);

            return value;
        }
    }
}
