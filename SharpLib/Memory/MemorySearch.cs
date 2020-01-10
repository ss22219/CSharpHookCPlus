using System;
using System.Collections.Generic;
using System.Linq;

namespace SharpLib.Memory
{
    public class MemorySearch
    {
        private byte[] _flags = null;
        private bool[] _isPramaters = null;
        private List<ByteParameter> _parameters = null;

        public static IntPtr[] FindPositions(string byteFlags, IntPtr ptr, uint length)
        {
            var ms = new MemorySearch(byteFlags);
            var result = ms.Find(ptr, length);
            if (!result.Success)
                return null;
            return result.ParamPositions.Select(p => ptr + p.Position).ToArray();
        }

        public static unsafe int[] FindPositions(string byteFlags, byte[] bytes, int length = -1)
        {
            var ms = new MemorySearch(byteFlags);
            var result = ms.Find(bytes, length);
            if (!result.Success)
                return null;
            return result.ParamPositions.Select(p =>  p.Position).ToArray();
        }

        public MemorySearch(string byteFlags)
        {
            Parse(byteFlags);
        }

        public unsafe MemorySearchResult Find(byte* ptr, uint length)
        {
            var result = new MemorySearchResult { Success = false, Index = -1 };
            if (_flags == null || _flags.Length == 0 || (IntPtr)ptr == IntPtr.Zero || length <= 0)
                return result;

            for (int i = 0; i < length; i++)
            {
                int j = 0;
                for (; j < _flags.Length; j++)
                {
                    if (_flags[j] != ptr[i + j] && !_isPramaters[j])
                        break;
                }

                if (j == _flags.Length)
                {
                    result.Index = i;
                    result.Success = true;
                    result.ParamPositions = GetParamPositions(i);
                    return result;
                }
            }
            return result;
        }

        private unsafe MemorySearchParamPosition[] GetParamPositions(int index)
        {
            if (_parameters == null)
                return new MemorySearchParamPosition[] { };
            var positions = new MemorySearchParamPosition[_parameters.Count];
            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = new MemorySearchParamPosition { Position = index + _parameters[i].Index };
            }
            return positions;
        }

        public unsafe MemorySearchResult Find(byte[] bytes, int length = -1)
        {
            if (length == -1)
                length = bytes.Length;
            fixed (byte* ptr = bytes)
                return Find(ptr, (uint)length);
        }

        public unsafe MemorySearchResult Find(IntPtr ptr, uint length)
        {
            return Find((byte*)ptr, length);
        }

        private void Parse(string byteFlags)
        {
            if (byteFlags == null)
                return;
            var words = byteFlags.Split(' ').Select(b => b.Trim()).Where(b => !string.IsNullOrEmpty(b)).ToArray();
            if (words.Length == 0)
                return;
            _parameters = new List<ByteParameter>();
            _flags = new byte[words.Length];
            _isPramaters = new bool[words.Length];
            ByteParameter parameter = null;
            for (int i = 0; i < words.Length; i++)
            {
                if (words[i] == "??")
                {
                    parameter = parameter ?? new ByteParameter { Index = i };
                    parameter.Length += 2;
                    if (i == 0 || words[i - 1] != "??")
                        _parameters.Add(parameter);
                    _isPramaters[i] = true;
                }
                else
                {
                    parameter = null;
                    _flags[i] = Convert.ToByte(words[i], 16);
                    _isPramaters[i] = false;
                }
            }
        }

        class ByteParameter
        {
            public int Index;
            public int Length;
        }
    }

    public class MemorySearchResult
    {
        public bool Success { get; internal set; }
        public int Index { get; internal set; }
        public MemorySearchParamPosition[] ParamPositions { get; internal set; }
    }

    public class MemorySearchParamPosition
    {
        public int Position { get; internal set; }
    }
}
