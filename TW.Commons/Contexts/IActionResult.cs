using TW.Commons.Contexts;

namespace UBS.IPV.Federal.Core.Data
{
    /// <summary>
    /// so that we got interface for Generic and non generic one
    /// </summary>
    public interface IActionResult
    {
        /// <summary>
        /// Some sort of Id/Key which can be used to link back to the ActionResult that is performed for it
        /// </summary>
        string Identifier { get; set; }

        ProcessStatus Status { get; set; }

        bool Failed { get; }

        bool Success { get; }

        string Message { get; set; }
    }
}