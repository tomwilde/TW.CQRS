using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Practices.Unity;
using Moq;

namespace TW.Commons.TestingUtilities.AutoMoqMocking
{
	public class AutoMockContainer : IDisposable
	{
		private readonly List<Mock> allCodedMocks = new List<Mock>();

		private readonly AutoMockingBuilderStrategy _strategy;

		readonly IUnityContainer _container;
		private bool allVerified;

		/// <summary>
		/// Initializes the container with the <see cref="MockFactory"/> that
		/// will be used to create dependent mocks.
		/// </summary>
		public AutoMockContainer()
		{
			_container = new UnityContainer();
			
			_strategy = new AutoMockingBuilderStrategy();
			_container.AddExtension(new AutoMockingContainerExtension(_strategy));

            //var _serviceLocator = RegisterMock<IServiceLocator>();
            //ServiceLocator.SetLocatorProvider(() => _serviceLocator.Object);
		}

		/// <summary>
		/// The actual container, exposed in case you need finer control.
		/// </summary>
		public IUnityContainer Object
		{
			get { return _container; }
		}

		public T Resolve<T>()
		{
			try
			{
				return _container.Resolve<T>();
			}
			catch (ResolutionFailedException ex)
			{
				if (ex.InnerException != null)
					if (ex.InnerException.InnerException != null)
						if (ex.InnerException.InnerException is ArgumentException)
							if (ex.InnerException.InnerException.Message == "Cannot set parent to an interface.")
								Debug.WriteLine("Known issue see: https://code.google.com/p/moq/issues/detail?id=259  (sorry folks)");

				throw;
			}
		}

		public T Resolve<T>(params ResolverOverride[] overrides)
		{
			try
			{
				return _container.Resolve<T>(overrides);
			}
			catch (ResolutionFailedException ex)
			{
				if (ex.InnerException != null)
					if (ex.InnerException.InnerException != null)
						if (ex.InnerException.InnerException is ArgumentException)
							if (ex.InnerException.InnerException.Message == "Cannot set parent to an interface.")
								Debug.WriteLine("Known issue see: https://code.google.com/p/moq/issues/detail?id=259  (sorry folks)");

				throw;
			}
		}

		/// <summary>
		/// Gets or creates a mock for the given type, with 
		/// the default behavior specified by the factory.
		/// </summary>
		public Mock<T> GetMock<T>() where T : class
		{
			var obj = (this.Create<T>() as IMocked<T>);

			try
			{
				return obj.Mock;
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("\r\nFailed to get mock:{0}\r\nHave you called container.RegisterMock<{0}>(); ?\r\n\r\n", typeof(T).Name), ex);
			}
		}

		/// <summary>
		/// Creates an instance of a class under test, 
		/// injecting all necessary dependencies as mocks.
		/// </summary>
		/// <typeparam name="T">Requested object type.</typeparam>
		public T Create<T>() where T : class
		{
			_strategy.SetCurrentType(typeof(T));

			try
			{
				return _container.Resolve<T>();
			}
			catch (Exception ex)
			{
				throw new Exception(string.Format("\r\nFailed to resolve type:{0}\r\nHave you called container.RegisterMock<{0}>(); ?\r\n\r\n", typeof(T).Name), ex);
			}
		}

		public Mock<T> RegisterMock<T>() where T : class
		{
			var mock = _container.RegisterMock<T>();
			allCodedMocks.Add(mock);
			return mock;
		}

		public Mock<T> ConfigureMockFor<T>() where T : class
		{
			// if the mock isnt alraedy registered, auto add it and log a warning...
			if (allCodedMocks.Find(mock => mock.Object is T) == null)
			{
                Debug.WriteLine(string.Format("WARN: Auto adding mock for type: {0}", typeof(T).Name));
                RegisterMock<T>();
			}

			return _container.Resolve<Mock<T>>();
		}

		public void VerifyMockFor<T>() where T : class
		{
			_container.VerifyMockFor<T>();
		}

		public void RegisterInstance<T>(T implementation)
		{
			_container.RegisterInstance(typeof (T), implementation);
		}

		public void RegisterConcreteService<F,T>() where T : F
		{
			_strategy.AddNonMockedType<F>();
			_container.RegisterType<F,T>();
		}

		public void RegisterConcreteService<F,T>(string key) where T : F
		{
			_strategy.AddNonMockedType<F>();
			_container.RegisterType<F,T>(key);
		}

		public void RegisterInstance<T>(T implementation, string name)
		{
			_container.RegisterInstance(GenerateNamedInstanceName<T>(name), implementation);
		}

		private string GenerateNamedInstanceName<T>(string name)
		{
			return string.Format("{0}_{1}", typeof(T), name);
		}

		public void VerifyAll()
		{
			_strategy.VerifyAutoGeneratedMocks();

			VerifyHandCodedMocks();
			allVerified = true;
		}

		private void VerifyHandCodedMocks()
		{
			allCodedMocks.ForEach(mock => mock.VerifyAll());
		}

		public T GetInstance<T>() where T : class
		{
			return _container.Resolve(typeof(T)) as T;
		}

		public T GetInstance<T>(string name) where T : class
		{
			return _container.Resolve<T>(GenerateNamedInstanceName<T>(name));
		}

		#region IDisposable implementation

		private bool disposed = false;

		public void Dispose()
		{
			Dispose(true);
			// This object will be cleaned up by the Dispose method.
			// Therefore, you should call GC.SupressFinalize to
			// take this object off the finalization queue 
			// and prevent finalization code for this object
			// from executing a second time.
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!this.disposed)
			{
				if (disposing)
				{
					// Dispose managed resources.
					if (!allVerified) VerifyAll();
				}
			}

			disposed = true;
		}

		#endregion

		public void RegisterConcreteType<T>()
		{
			_strategy.AddNonMockedType<T>();
			_container.RegisterType<T>();
		}
	}

}
