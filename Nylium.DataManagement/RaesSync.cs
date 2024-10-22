using DevExpress.ExpressApp.EFCore;
using DevExpress.ExpressApp;
using Nylium.Module.BusinessObjects;
using Microsoft.EntityFrameworkCore;
using Nylium.Module.Raes.BusinessObjects;
using Nylium.Module.Lab.BusinessObjects.Project;
using Trinity.SqlServer;
using System.Data;

namespace Nylium.DataManagement
{
    public class RaesSync
    {
        private const string sourceConnectionString = "Integrated Security=SSPI;Data Source=localhost;Initial Catalog=RAES041024;TrustServerCertificate=True";
        private const string destinationConnectionstring = "Integrated Security=SSPI;Data Source=localhost;Initial Catalog=RAESLABDB;TrustServerCertificate=True";
        SqlServerDataManager<ProjectBNO> projectManager

        private SqlServerDataContext SqlServerDataContext { get; set; }


        public IObjectSpace GetObjectSpace()
        {
            //var connection_string = System.Configuration.ConfigurationManager.ConnectionStrings["DebugConnectionString"].ConnectionString;
            //var connection_string = "Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=EFCoreTestDB;Integrated Security=True;MultipleActiveResultSets=True";




            var objectSpaceProvider = new EFCoreObjectSpaceProvider<RaesDbContext>(
                (builder, _) => builder
                    .UseSqlServer(destinationConnectionstring)
                    .UseLazyLoadingProxies()
                    .UseChangeTrackingProxies()
            );

            IObjectSpace objectSpace = objectSpaceProvider.CreateObjectSpace();
            return objectSpace;
        }


        public void CreateDataContext()
        {
            SqlServerDataContext = new SqlServerDataContext(sourceConnectionString);
        }



        public void SyncProjects()
        {

            CreateDataContext();

            var objectSpace = GetObjectSpace();
            projectManager = new SqlServerDataManager<ProjectBNO>(sourceConnectionString);



            var destManager = new SqlServerDataManager<ProjectTypeBNO>(sourceConnectionString);

            var projectTypeManager = new SqlServerDataManager<dynamic>(sourceConnectionString);

            //var allData = dataManager.Select().From("ProjectBNO").All().ExecuteToList();

            var allData = dataManager.Select().All().ExecuteToList();
            var typeList = projectTypeManager.Select().From("BaseEntityTypeBNO").All().ExecuteToList();


            foreach (var item in typeList)
            {
                destManager.Insert(item).InTo("BaseEntityTypeBNO");
            }

            var result = destManager.Execute();

            //var clonnert = new NyliumObjectClonerFactory<ProjectTypeBNO>().CloneCreateObjectList(objectSpace, typeList);

            var clonner = new NyliumObjectClonerFactory<ProjectBNO>().CloneCreateObjectList(objectSpace, allData);




            objectSpace.CommitChanges();






        }



    }
}
