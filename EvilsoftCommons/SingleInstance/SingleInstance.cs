using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;

namespace EvilsoftCommons.SingleInstance {

    /// <summary>This class ensures there is only one instance of this program running at any given time.
    /// Running multiple instances could result in item duplication </summary>
    public class SingleInstance : IDisposable {
        static ILog logger = LogManager.GetLogger("SingleInstance");
        private Mutex mutex = null;
        private Boolean ownsMutex = false;
        private Guid identifier = Guid.Empty;

        /// <summary>
        /// Enforces single instance for an application.
        /// </summary>
        /// <param name="identifier">An identifier unique to this application.</param>
        public SingleInstance(Guid identifier) {
            this.identifier = identifier;
            mutex = new Mutex(true, identifier.ToString(), out ownsMutex);
        }

        /// <summary>
        /// Indicates whether this is the first instance of this application.
        /// </summary>
        public Boolean IsFirstInstance { get { return ownsMutex; } }

        /// <summary>
        /// Passes the given arguments to the first running instance of the application.
        /// </summary>
        /// <param name="arguments">The arguments to pass.</param>
        /// <returns>Return true if the operation succeded, false otherwise.</returns>
        public Boolean PassArgumentsToFirstInstance(String[] arguments) {
            if (IsFirstInstance)
                throw new InvalidOperationException("This is the first instance.");

            try {
                using (NamedPipeClientStream client = new NamedPipeClientStream(identifier.ToString()))
                using (StreamWriter writer = new StreamWriter(client)) {
                    client.Connect(200);

                    foreach (String argument in arguments)
                        writer.WriteLine(argument);
                }
                return true;
            }
            catch (TimeoutException) { } //Couldn't connect to server
            catch (IOException) { } //Pipe was broken

            return false;
        }

        /// <summary>
        /// Listens for arguments being passed from successive instances of the application.
        /// </summary>
        public void ListenForArgumentsFromSuccessiveInstances() {
            if (!IsFirstInstance)
                throw new InvalidOperationException("This is not the first instance.");
        }


        #region IDisposable
        private Boolean disposed = false;

        protected virtual void Dispose(bool disposing) {
            if (!disposed) {
                if (mutex != null && ownsMutex)
                    try {
                        mutex.ReleaseMutex();
                        mutex = null;
                    }
                    catch (System.ApplicationException) {
                        // Were shutting down, so just eat it and move on.
                    }
                    catch (Exception ex) {
                        logger.Warn(ex.Message);
                    }
                disposed = true;
            }
        }

        ~SingleInstance() {
            Dispose(false);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
