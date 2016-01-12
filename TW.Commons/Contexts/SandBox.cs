using System;
using TW.Commons.Interfaces;
using UBS.IPV.Federal.Core.Data;

namespace UBS.Ipv.Evita.BidOfferGlobal.BusinessLogic.V2
{
    // TODO: tests

    public interface ISandBox
    {
        void ExecuteInSandbox(string logMsg, Action func);
        ActionResult<T> ExecuteInSandbox<T>(string logMsg, Func<T> func);
        ActionResult ExecuteInSandbox<S>(string logMsg, S data, Action<S> func);
        ActionResult ExecuteInSandbox<S>(string logMsg, S data, Func<S, ActionResult> func);
        ActionResult<T> ExecuteInSandbox<S, T>(string logMsg, S data, Func<S, T> func);
        ActionResult<T> ExecuteInSandbox<S, T>(string logMsg, S data, Func<S, ActionResult<T>> func);
        ActionResult<T> ExecuteInSandbox<L, R, T>(string logMsg, L left, R right, Func<L, R, T> func);
        ActionResult<T> ExecuteInSandbox<L, R, T>(string logMsg, L left, R right, Func<L, R, ActionResult<T>> func);
    }

    public class SandBox : ISandBox
    {
        private readonly ILogger _logger;

        public SandBox(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.Create();
        }

        public void ExecuteInSandbox(string logMsg, Action func)
        {
            _logger.Info(logMsg);

            try
            {
                func();
                _logger.InfoFormat(logMsg + " [Done]");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }

        public ActionResult<T> ExecuteInSandbox<T>(string logMsg, Func<T> func)
        {
            _logger.Info(logMsg);

            var actionResult = new ActionResult<T>();

            try
            {
                actionResult.Complete(func());

                _logger.InfoFormat(logMsg + " [Done]");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                actionResult.Fail("Unhandled Exception! {0}", ex.Message);
            }

            return actionResult;
        }

        public ActionResult ExecuteInSandbox<S>(string logMsg, S data, Action<S> func)
        {
            _logger.Info(logMsg);

            var actionResult = new ActionResult();

            try
            {
                func(data);

                actionResult.Complete();

                _logger.InfoFormat(logMsg + " [Done]");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                actionResult.Fail("Unhandled Exception! {0}", ex.Message);
            }

            return actionResult;
        }

        public ActionResult ExecuteInSandbox<S>(string logMsg, S data, Func<S, ActionResult> func)
        {
            _logger.Info(logMsg);

            try
            {
                var result = func(data);
                _logger.InfoFormat(logMsg + " [Done]");
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return new ActionResult().Fail("Unhandled Exception! {0}", ex.Message);
            }
        }

        public ActionResult<T> ExecuteInSandbox<S, T>(string logMsg, S data, Func<S, T> func)
        {
            var actionResult = new ActionResult<T>();
            _logger.Info(logMsg);

            try
            {
                actionResult.Complete(func(data));

                _logger.InfoFormat(logMsg + " [Done]");
                return actionResult;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return new ActionResult<T>().Fail("Unhandled Exception! {0}", ex.Message);
            }
        }

        public ActionResult<T> ExecuteInSandbox<S, T>(string logMsg, S data, Func<S, ActionResult<T>> func)
        {
            _logger.Info(logMsg);

            try
            {
                var actionResult = func(data);
                _logger.InfoFormat(logMsg + " [Done]");
                return actionResult;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return new ActionResult<T>().Fail("Unhandled Exception! {0}", ex.Message);
            }
        }

        public ActionResult<T> ExecuteInSandbox<R, S, T>(string logMsg, R left, S right, Func<R, S, T> func)
        {
            _logger.Info(logMsg);

            var actionResult = new ActionResult<T>();

            try
            {
                actionResult.Complete(func(left, right));

                _logger.InfoFormat(logMsg + " [Done]");
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                actionResult.Fail("Unhandled Exception! {0}", ex.Message);
            }

            return actionResult;
        }

        public ActionResult<T> ExecuteInSandbox<R, S, T>(string logMsg, R left, S right, Func<R, S, ActionResult<T>> func)
        {
            _logger.Info(logMsg);

            try
            {
                var result = func(left, right);
                _logger.InfoFormat(logMsg + " [Done]");

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                return new ActionResult<T>().Fail("Unhandled Exception! {0}", ex.Message);
            }
        }
    }
}