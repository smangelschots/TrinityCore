using System;
using System.Collections.Generic;
using System.Linq;
using Trinity.DataAccess.Interfaces;
using Trinity.DataAccess.Logging;
using Trinity.DataAccess.Orm;

namespace Trinity.DataAccess.Results
{




    //TODO Rewrite
    public class CommandResult : ICommandResult
    {
        public List<DataError> CommandErrors { get; set; }
        public List<string> Messages { get; set; }

        public void AddError(LogType errorType, string message, Exception exception = null)
        {
            this.CommandErrors.Add(new DataError()
            {
                ErrorType = errorType,
                Message = message,
                Exception = exception,
                HasError = true
            });

            var sSource = "Datamanager";
            var sLog = "Datamanager";
            var sEvent = message;

            if (exception != null)
                sEvent += exception.Message + " " + exception.StackTrace;

            LoggingService.SendToLog(sLog, sEvent, errorType);


        }

        public void AddMessage(string message)
        {
            this.Messages.Add(message);
            LoggingService.SendToLog("Datamanager", message, LogType.Information);
        }

        public void AddWaring(string message, Exception ex = null)
        {
            this.Messages.Add(message);
            LoggingService.SendToLog("Datamanager", message + " " + ex.ToString(), LogType.Warning);
        }


        public int RecordsAffected { get; set; }

        public bool HasErrors
        {
            get
            {
                return this.CommandErrors.Any(m => m.HasError);
            }
        }

        public DataCommandType CommandType
        {
            get
            {
                if (DataCommand != null)
                    return DataCommand.CommandType;

                return DataCommandType.Unknown;
            }
        }

        public IDataCommand DataCommand { get; set; }
        public long ElapsedMilliseconds { get; set; }



        public bool IsMapped
        {
            get
            {
                if (DataCommand != null)
                {
                    if (DataCommand.TableMap != null)
                    {
                        if (DataCommand.TableMap.Mapped && DataCommand.TableMap.ColumnsMapped)
                        {
                            return true;
                        }

                    }
                }

                return false;
            }
        }


        public CommandResult()
        {
            this.Messages = new List<string>();
            this.CommandErrors = new List<DataError>();
        }

    }
}