using Microsoft.Extensions.Configuration;
using PersonReader.Interface;
using System.IO;
using System.Linq;
using System.Reflection;
using System;

namespace PersonReader.Factory
{
    public class ReaderFactory
    {
        private IConfiguration Configuration; 

        public ReaderFactory(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IPersonReader? reader;

        public IPersonReader GetReader()
        {
            if (reader != null)
                return reader;

            // Check configuration
            string? readerAssemblyName = Configuration["DataReader:ReaderAssembly"];
            string readerLocation = AppDomain.CurrentDomain.BaseDirectory 
                                    + "ReaderAssemblies" 
                                    + Path.DirectorySeparatorChar 
                                    + readerAssemblyName;
            // Load the assembly
            ReaderLoadContext loadContext = new ReaderLoadContextContext(readerLocation);
            AssemblyName assemblyName = new AssemblyName(Path.GetFileNameWithoutExtension(readerLocation));
            Assembly readerAssembly = loadContext.LoadFromAssemblyNAme(assemblyName);

            //Look for the type
            string? readerTypeName = Configuration["DataReader:RenderType"];
            Type readerType = readerAssembly.ExportedTypes.First(t => t.FullName == readerTypeName);

            //Create the data reader
            reader = Activator.CreateInstance(readerType) as IPersonReader;
            if (reader is null)
            {
                throw new InvalidOperationException(
                    $"Unable to create instance of {readerType} as IpersonReader");
            }

        //public IPersonReader GetReader(string readerType)
        //{
        //    switch (readerType)
        //    {
        //        case "Service": return new ServiceReader();
        //        case "CSV": return new CSVReader();
        //        case "SQL": return new SQLReader();
        //        default:
        //            throw new ArgumentException($"Invalid reader type: {readerType}");
        //    }
            //Return the data reader
            return reader;
        }
    }
}
