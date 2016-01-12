using System;
using System.Linq;
using TW.Commons.Contexts;
using TW.Commons.Interfaces;

namespace UBS.IPV.Federal.Core.Data
{
    /// <summary>
    /// Use this class if you want to return additonal metadata (status/message) when certain Action goes wrong along with Result if all goes well
    /// (this class have been created to avoid "Exception drived code") (kind of monad)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class ActionResult<T> : ActionResultBase, IActionResult
    {
        /// <summary>
        /// Result of this given Action
        /// </summary>
        public T Result { get; set; }

        #region Ctor

        public ActionResult()
        {

        }

        public ActionResult(string identifier)
        {
            Identifier = identifier;
        }

        public ActionResult(ProcessStatus status, string message)
            : this(status, message, default(T))
        {
        }

        public ActionResult(string identifier, ProcessStatus status, string message)
            : this(status, message, default(T))
        {
            Identifier = identifier;
        }

        public ActionResult(ProcessStatus status, string message, T result)
        {
            Status = status;
            Message = message;
            Result = result;
        }

        #endregion

        #region SetStatus Related

        public ActionResult<T> Start()
        {
            Status = ProcessStatus.Started;
            //fluent shmuent
            return this;
        }

        /// <summary>
        /// Fails the Process by settings the Status to Failed
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public ActionResult<T> Fail(string message, params object[] args)
        {
            if (!string.IsNullOrEmpty(message))
            {
                SetStatus(ProcessStatus.Failed, args != null ? args.Any() ? string.Format(message, args) : message : message);
            }
            else
            {
                Status = ProcessStatus.Failed;
            }

            //fluent shmuent
            return this;
        }

        /// <summary>
        /// Fails the Process by settings the Status to Failed and provide additional identifier
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public ActionResult<T> FailWithId(string identifier, string message, params object[] args)
        {
            SetStatus(identifier, ProcessStatus.Failed, message, args);
            //fluent shmuent
            return this;
        }

        public ActionResult<T> SetStatus(ProcessStatus status, string message)
        {
            Status = status;

            if (message != null)
            {
                Message = message;
            }

            //fluent shmuent
            return this;
        }

        /// <summary>
        /// Completes the current process (sets to Success)
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public ActionResult<T> Complete(T result)
        {
            Result = result;
            SetStatus(ProcessStatus.Complete, "Successfully Complete");
            return this; //fluent shmuent
        }

        /// <summary>
        /// Completes the current process (sets to Success)
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public ActionResult<T> CompleteWithId(string identifier, T result)
        {
            Identifier = identifier;
            Result = result;
            SetStatus(ProcessStatus.Complete, "Successfully Complete");
            return this; //fluent shmuent
        }


        /// <summary>
        /// Completes the current process (sets to Success)
        /// </summary>
        /// <param name="result"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public ActionResult<T> Complete(T result, string message, params object[] args)
        {
            Result = result;
            SetStatus(ProcessStatus.Complete, string.Format(message, args));
            return this; //fluent shmuent
        }

        /// <summary>
        /// Completes the current process (sets to Success)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public ActionResult<T> Complete(string message, params object[] args)
        {
            SetStatus(ProcessStatus.Complete, string.Format(message, args));
            return this; //fluent shmuent
        }

        #endregion

        public ActionResult<T> Log(ILogger logger)
        {
            LogMessage(logger);
            return this;
        }

        #region Accept Transfer

        public ActionResult<T> AcceptTransfer<TT>(ActionResult<TT> transferredFrom, string identifier = null)
        {
            AcceptTransfer((IActionResult)transferredFrom, identifier);
            return this;
        }

        public ActionResult<T> AcceptTransfer(ActionResult transferredFrom, string identifier = null)
        {
            AcceptTransfer((IActionResult)transferredFrom, identifier);
            return this;
        }


        #endregion

        public override string ToString()
        {
            return string.Format("Status:{0}, Message:{1}", Status, Message);
        }

        #region Static Helpers

        public static ActionResult<T> CreateSuccess(T result, string message = "Successfully Complete")
        {
            return new ActionResult<T>(ProcessStatus.Complete, message, result);
        }

        public static ActionResult<T> CreateSuccess(T result, IActionResult actionResult)
        {
            return new ActionResult<T>(actionResult.Identifier, ProcessStatus.Complete, actionResult.Message)
                {
                    Result = result,
                };
        }

        public static ActionResult<T> CreateFail(string message, params object[] args)
        {
            var finalMessage = message;
            if (args != null)
            {
                finalMessage = string.Format(message, args);
            }
            return new ActionResult<T>(ProcessStatus.Failed, finalMessage);
        }

        public static ActionResult<T> CreateFail(IActionResult actionResult)
        {
            return new ActionResult<T>(actionResult.Identifier, ProcessStatus.Failed, actionResult.Message);
        }

        public static ActionResult<T> CreateFailWithId(string identifier, string message, params object[] args)
        {
            var finalMessage = message;
            if (args != null)
            {
                finalMessage = string.Format(message, args);
            }
            return new ActionResult<T>(ProcessStatus.Failed, finalMessage);
        }

        #endregion

        /// <summary>
        /// Tries to perform action within try catch block and set Status/Message and Result accordingly
        /// </summary>
        /// <param name="funcSelector"></param>
        public ActionResult<T> TryPerformAction(Func<T> funcSelector)
        {
            try
            {
                var result = funcSelector();
                Complete(result);
            }
            catch (Exception exc)
            {
                Fail("Failed with Error:{0}", exc.Message);
            }

            return this;
        }
    }
}
