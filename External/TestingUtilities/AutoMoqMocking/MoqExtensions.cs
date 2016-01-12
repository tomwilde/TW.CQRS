using System.Collections.Generic;
using Moq.Language.Flow;

namespace TW.Commons.TestingUtilities.AutoMoqMocking
{
    public static class MoqExtensions
    {
        /// <summary>
        /// Provides some ordering support
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="setup"></param>
        /// <param name="results"></param>
        public static void ReturnsInSuccession<T, TResult>(this ISetup<T, TResult> setup, params TResult[] results) where T : class
        {
            setup.Returns(new Queue<TResult>(results).Dequeue);
        }
    }
}