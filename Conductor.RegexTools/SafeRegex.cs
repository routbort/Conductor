using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Conductor.RegexTools

{

    [Serializable]
    public class RegexPerformanceException : Exception
    {
        public string ResourceReferenceProperty { get; set; }

        public RegexPerformanceException()
        {
        }

        public RegexPerformanceException(string message)
            : base(message)
        {
        }

        public RegexPerformanceException(string message, Exception inner)
            : base(message, inner)
        {
        }

        protected RegexPerformanceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            ResourceReferenceProperty = info.GetString("ResourceReferenceProperty");
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            info.AddValue("ResourceReferenceProperty", ResourceReferenceProperty);
            base.GetObjectData(info, context);
        }

    }

    /// <summary>
    /// Provides timeout functions for Regex to protect against catastrophic backtracking.
    /// The underlying Regex (BaseRegex) is retrievable for convenience, but methods called directly are not protected.
    /// </summary>
    public class SafeRegex
    {

        public int Timeout { get; set; } = 4000;

        public Regex InternalRegex { get { return _Regex; } }

        Regex _Regex = null;

        private SafeRegex()
        {


        }

        /// <summary>
        /// Initializes and compiles an instance of the SafeRegex class for the specified regular expression.
        /// </summary>
        /// <param name="pattern">The regular expression pattern to match</param>
        public SafeRegex(string pattern)
        {
            _Regex = new Regex(pattern);
        }

        /// <summary>
        /// Initializes and compiles an instance of the SafeRegex class for the specified regular expression
        /// </summary>
        /// <param name="pattern">The regular expression pattern to match</param>
        /// <param name="timeout">Timeout in milliseconds - if matching takes longer, exception will be thrown</param>
        public SafeRegex(string pattern, int timeout)
        {
            _Regex = new Regex(pattern);
            Timeout = timeout;
        }

        /// <summary>
        /// Initializes and compiles an instance of the SafeRegex class for the specified regular expression
        /// </summary>
        /// <param name="pattern">The regular expression pattern to match</param>
        /// <param name="timeout">Timeout in milliseconds - if matching takes longer, exception will be thrown</param>
        /// <param name="options">A bitwise combination of the enumeration values that modify matching behavior</param>
        public SafeRegex(string pattern, int timeout, RegexOptions options)
        {
            _Regex = new Regex(pattern, options);
            Timeout = timeout;
        }

        public MatchCollection Matches(string input)
        {
            _LastMatchExecutionTime = null;
            Stopwatch sw = new Stopwatch();
            sw.Start();
            var workThread = new Thread(new ParameterizedThreadStart(SafeRegexWorkerThread));
            workThread.Start(input);
            if (!workThread.Join(Timeout))
            {
                workThread.Abort();
                throw new RegexPerformanceException("Timeout of " + Timeout.ToString() + " ms exceeded - probable catastrophic backtracking");
            }
            sw.Stop();
            _LastMatchExecutionTime = sw.ElapsedMilliseconds;
            return _matches;
        }

        public Regex BaseRegex { get { return _Regex; } }

        long? _LastMatchExecutionTime = null;
        public long? LastMatchExecutionTime { get { return _LastMatchExecutionTime; } }

        private void SafeRegexWorkerThread(object argument)
        {
            MatchCollection matches = _Regex.Matches(argument.ToString());
            int matchCount = matches.Count;
            _matches = matches;
        }

        MatchCollection _matches = null;



    }
}
