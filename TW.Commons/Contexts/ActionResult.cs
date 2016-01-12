using System;
using TW.Commons.Contexts;
using TW.Commons.Interfaces;

namespace UBS.IPV.Federal.Core.Data
{
    /// <summary>
    /// This is class is used to encapsulate Result of the performed action
    /// (this class have been created to avoid "Exception drived code") (kind of monad)
    /// </summary>
    [Serializable]
    public class ActionResult : ActionResultBase
    {
        #region Ctor

        public ActionResult()
        {

        }

        public ActionResult(string identifier)
        {
            Identifier = identifier;
        }

        public ActionResult(ProcessStatus status, string message = null)
        {
            Status = status;
            Message = message;
        }

        #endregion

        #region SetStatus Related

        public ActionResult Start()
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
        public ActionResult Fail(string message, params object[] args)
        {
            SetStatus(ProcessStatus.Failed, string.Format(message, args));
            //fluent shmuent
            return this;
        }

        /// <summary>
        /// Fails the Process by settings the Status to Failed
        /// </summary>
        /// <returns></returns>
        public ActionResult Fail(Exception exc)
        {
            return Fail("Failed operation with Error:{0}", exc.Message);
        }

        public ActionResult SetStatus(ProcessStatus status, string message)
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
        /// <returns></returns>
        public ActionResult Complete()
        {
            SetStatus(ProcessStatus.Complete, "Successfully Complete");
            return this; //fluent shmuent
        }

        /// <summary>
        /// Completes the current process (sets to Success)
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public ActionResult Complete(string message, params object[] args)
        {
            SetStatus(ProcessStatus.Complete, string.Format(message, args));
            return this; //fluent shmuent
        }

        #endregion

        public ActionResult Log(ILogger logger)
        {
            LogMessage(logger);
            return this;
        }

        public ActionResult AcceptTransfer(ActionResult instanceResult)
        {
            AcceptTransfer((IActionResult)instanceResult);
            return this;
        }

        public ActionResult AcceptTransfer<T>(ActionResult<T> instanceResult)
        {
            AcceptTransfer((IActionResult)instanceResult);
            return this;
        }


        #region Static Helpers

        public static ActionResult CreateSuccess(string message = "Successfully Complete")
        {
            return new ActionResult(ProcessStatus.Complete, message);
        }

        public static ActionResult CreateFail(string message, params object[] args)
        {
            var finalMessage = message;
            if (args != null)
            {
                finalMessage = string.Format(message, args);
            }
            return new ActionResult(ProcessStatus.Failed, finalMessage);
        }

        #endregion

        public override string ToString()
        {
            return string.Format("Status:{0}, Message:{1}", Status, Message);
        }
    }
}

