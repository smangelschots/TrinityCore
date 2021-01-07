using NUnit.Framework;
using System;
using System.Data;
using Trinity.DataAccess.Attributes;
using Trinity.DataAccess.Events;
using Trinity.SqlServer;

namespace Trinity.Test
{
    public class Tests
    {


        private string testTableName = "TestTable";
        private string connection = "Server=localhost;Database=Trinity;Trusted_Connection=yes; Application Name=Trinity;";
        private SqlServerDataContext dataContext;

        public class TestTableNoMap
        {
            public Guid Id { get; set; }
            public string Name { get; set; }
        }

        [TableConfiguration("TestTable")]
        public class TestTableMapped
        {
            [ColumnConfiguration("id")]
            public Guid Id { get; set; }
            public string Name { get; set; }
        }



        [SetUp]
        public void Setup()
        {
            dataContext = new SqlServerDataContext(connection);
            dataContext.Init();
        }

        [Test]
        public void NoModelNoTableTest()
        {
            var manager = new SqlServerDataManager<DataTable>(connection);

            manager.CreateUpdateTable(manager.CreateTableMap<SqlColumnMap>("TestCreateTable").MapColumn("id", "Id", (e) =>
            {
                e.DbType = SqlDbType.UniqueIdentifier;
                e.IsPrimaryKey = true;
            }).MapColumn("Name", "Name", (e) => {
                e.DbType = SqlDbType.NVarChar;
                e.Size = 255;
            })
);




            manager.Execute();




            Assert.Pass();
        }

        [Test]
        public void NoModelTableFirstTest()
        {
            var manager = new SqlServerDataManager<DataTable>(connection);
            var map = manager.CreateTableMap<SqlColumnMap>("TestTable")
                .MapColumn("id","Id").MapColumn("Name","Name");

            var result = manager.Select().From(map.TableName).Execute();

            Assert.IsTrue(result.HasErrors);
            Assert.IsTrue(result.RecordsAffected > 0);
        }

        [Test]
        public void Model2TableMissingMapInsertTest()
        {
            var manager = new SqlServerDataManager<TestTableNoMap>(connection);

            manager.AfterGetTableMap += (object sender, ModelCommandAfterGetTableMapEventArgs e) =>
            {
                var map = e.TableMap;
                map.MapColumn("id", (e) => {
                    if (e == null)
                    {
                        e = new SqlColumnMap();
                        e.PropertyName = "Id";

                    }
                    else
                    {
                        e.PropertyName = "Id";
                    }
                    return e;
                });
            };
            var command = manager.Insert(new TestTableNoMap()
            {
                Id = Guid.NewGuid(),
                Name = $"Model2TableNoMapInsertTest"
            }).InTo("TestTable");

            manager.Execute();
            Assert.Pass();
        }



        [Test]
        public void Model2TableMappedInsertTest()
        {
            var manager = new SqlServerDataManager<TestTableMapped>(connection);
            for (int i = 0; i < 100; i++)
            {
                var command = manager.Insert(new TestTableMapped()
                {
                    Id = Guid.NewGuid(),
                    Name = $"Mapped Value {i}"
                }).InTo("TestTable");

               //command2.SetValue("Name", $"Value 1.{i}");
            }

            manager.Execute();
            Assert.Pass();
        }

        [Test]
        public void DataSelectTest()
        {
            Assert.Pass();
        }


        [Test]
        public void DataInsertTest()
        {
            var manager = new SqlServerDataManager<TestTableNoMap>(connection);


            for (int i = 0; i < 100; i++)
            {
                var command = manager.Insert(new TestTableNoMap()
                {
                    Id = Guid.NewGuid(),
                    Name = $"Value {i}"
                }).InTo("TestTable");


                var command2 = manager.Insert().InTo("TestTable");

                //command2.SetValue("Name", $"Value 1.{i}");
            }

           var result = manager.Execute();



            Assert.Pass();
        }

        [Test]
        public void DataUpdateTest()
        {
            Assert.Pass();
        }

        [Test]
        public void DataDeleteTest()
        {
            Assert.Pass();
        }


    }
}