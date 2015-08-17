using ServiceLibrary;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace WebApplication
{
    public class DBNoLock : stockdbaEntities
    {
        public DBNoLock() {
            // Get the ObjectContext related to this DbContext
            var objectContext = (this as IObjectContextAdapter).ObjectContext;

            // Sets the command timeout for all the commands
            objectContext.CommandTimeout = 300;

            objectContext.Connection.Open();
            objectContext.Connection.BeginTransaction(System.Data.IsolationLevel.ReadUncommitted);
        }
    }
}