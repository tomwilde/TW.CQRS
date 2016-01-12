using System;
using System.Linq;
using TW.Commons.Contexts;
using TW.Commons.Interfaces;

namespace UBS.IPV.Federal.Core.Data
{
    /// <summary>
    /// to merge Generic and nonGen implementation
    /// </summary>
    [Serializable]
    public abstract class ActionResultBase : IActionResult
    {
        /// <summary>
        /// Some sort of Id/Key which can be used to link back to the ActionResult that is performed for it
        /// </summary>
        public string Identifier { get; set; }

        public ProcessStatus Status { get; set; }

        /// <summary>
        /// Flag which returns true if Status is Complete/CompleteWithAction, otherwise false
        /// </summary>
        public bool Success { get { return Status == ProcessStatus.Successful; } }

        /// <summary>
        /// !Success
        /// </summary>
        public bool Failed { get { return !Success; } }

        public string Message { get; set; }
        
        internal void LogMessage(ILogger logger)
        {
            if (string.IsNullOrEmpty(Message)) return;

            if (Status == ProcessStatus.Failed)
            {
                logger.ErrorFormat("{0}{1}", Message, Identifier != null ? "Identifier:" + Identifier + ", " : null);
            }
            else
            {
                logger.InfoFormat("{0}{1}", Message, Identifier != null ? "Identifier:" + Identifier + ", " : null);
            }
        }

        internal void SetStatus(string identifier, ProcessStatus status, string message, params object[] args)
        {
            if (identifier != null && Identifier != null) //if not set already and then what you want to set is not null
            {
                Identifier = identifier;
            }
            Status = status;

            Message = args == null || !args.Any() ? string.Format(message, args) : message;
        }
        
        public void AcceptTransfer(IActionResult transferredFrom, string identifier = null)
        {
            SetStatus(identifier, transferredFrom.Status, transferredFrom.Message);
        }
    }
}