// -----------------------------------------------------------------------
//  <copyright file="DatabaseTests.cs" company="Prism">
//  Copyright (c) Prism. All rights reserved.
//  </copyright>
// -----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;
using Xunit;

namespace Prism.Picshare.Data.LiteDB.Tests;

public class DatabaseTests
{
    [Fact]
    public void Count()
    {
        using var db = GetFakeDatabase();
        var count = db.Count<DummyModel>();
        Assert.Equal(6, count);
    }

    [Fact]
    public void CountPredicate()
    {
        using var db = GetFakeDatabase();
        var count = db.Count<DummyModel>(x => x.Lastname == "Macon");
        Assert.Equal(2, count);
    }

    [Fact]
    public void Delete()
    {
        using var db = GetFakeDatabase();
        var deleted = db.Delete<DummyModel>(new Guid("5683855E-B07A-0582-C577-3A1C02FBD992"));
        var count = db.Count<DummyModel>();
        Assert.Equal(5, count);
        Assert.True(deleted);
    }

    [Fact]
    public void Delete_Not_Found()
    {
        using var db = GetFakeDatabase();
        var deleted = db.Delete<DummyModel>(new Guid("5683855E-B07A-0582-C577-3A1C02FBD993"));
        var count = db.Count<DummyModel>();
        Assert.Equal(6, count);
        Assert.False(deleted);
    }

    [Fact]
    public void Delete_Many()
    {
        using var db = GetFakeDatabase();
        var deleted = db.DeleteMany<DummyModel>(x => x.Lastname == "Macon");
        var count = db.Count<DummyModel>();
        Assert.Equal(4, count);
        Assert.Equal(2, deleted);
    }

    [Fact]
    public void Exists_Yes()
    {
        using var db = GetFakeDatabase();
        var exists = db.Exists<DummyModel>(x => x.Lastname == "Macon");
        Assert.True(exists);
    }

    [Fact]
    public void Exists_No()
    {
        using var db = GetFakeDatabase();
        var exists = db.Exists<DummyModel>(x => x.Lastname == "Maron");
        Assert.False(exists);
    }

    [Fact]
    public void Find_Paged()
    {
        using var db = GetFakeDatabase();
        var found = db.Find<DummyModel>(x => x.Lastname.StartsWith("M"), 1, 1);
        Assert.Single(found);
    }

    [Fact]
    public void Find_Paged_All()
    {
        using var db = GetFakeDatabase();
        var found = db.Find<DummyModel>(x => x.Lastname.StartsWith("M"));
        Assert.Equal(3, found.Count());
    }

    [Fact]
    public void Find_Paged_Skipped()
    {
        using var db = GetFakeDatabase();
        var found = db.Find<DummyModel>(x => x.Lastname.StartsWith("M"), 1);
        Assert.Equal(2, found.Count());
    }

    [Fact]
    public void Find_All()
    {
        using var db = GetFakeDatabase();
        var found = db.FindAll<DummyModel>();
        Assert.Equal(6, found.Count());
    }

    [Fact]
    public void Find_ById_Found()
    {
        using var db = GetFakeDatabase();
        var found = db.FindById<DummyModel>(new Guid("5683855E-B07A-0582-C577-3A1C02FBD992"));
        Assert.Equal("Macon", found.Lastname);
        Assert.Equal("Roger", found.Firstname);
    }

    [Fact]
    public void Find_ById_NotFound()
    {
        using var db = GetFakeDatabase();
        var found = db.FindById<DummyModel>(new Guid("5683855E-B07A-0582-C577-3A1C02FBD942"));
        Assert.Null(found);
    }

    [Fact]
    public void Find_One()
    {
        using var db = GetFakeDatabase();
        var found = db.FindOne<DummyModel>(x => x.Lastname == "Brendan");
        Assert.Equal("Brendan", found.Lastname);
        Assert.Equal("Zamora", found.Firstname);
    }

    [Fact]
    public void Insert_Enumerable()
    {
        using var db = GetFakeDatabase();

        var inserted = db.Insert(new List<DummyModel>
        {
            new DummyModel(new Guid("E762DDCE-4DEE-0F25-751D-F9019DC3462A"), "Brendan", "Zamora", "augue.malesuada@aol.org", 42),
            new DummyModel(new Guid("5683855E-B07A-0582-C577-3A1C02FBD99B"), "Macon", "Wright", "gravida@aol.org", 42),
            new DummyModel(new Guid("5683855E-B07A-0582-C577-3A1C02FBD99C"), "Macon", "Roger", "roger@aol.org", 42),
            new DummyModel(new Guid("5C7164EE-ACCD-828B-BA63-622E3B72E11D"), "Mercedes", "Martinez", "nullam.scelerisque@protonmail.net", 42),
            new DummyModel(new Guid("E0C463C0-176D-154C-5E27-4B5EA485A22E"), "Velma", "Gutierrez", "ullamcorper@aol.couk", 42),
            new DummyModel(new Guid("A34629FF-3F15-4B1D-1C67-15401E18EC4F"), "Brittany", "Mccoy", "et.rutrum@yahoo.net", 42)
        });

        Assert.True(inserted);

        var count = db.Count<DummyModel>();
        Assert.Equal(12, count);
    }

    [Fact]
    public void Insert_Bulk()
    {
        using var db = GetFakeDatabase();

        var inserted = db.InsertBulk(new List<DummyModel>
        {
            new DummyModel(new Guid("E762DDCE-4DEE-0F25-751D-F9019DC3462A"), "Brendan", "Zamora", "augue.malesuada@aol.org", 42),
            new DummyModel(new Guid("5683855E-B07A-0582-C577-3A1C02FBD99B"), "Macon", "Wright", "gravida@aol.org", 42),
            new DummyModel(new Guid("5683855E-B07A-0582-C577-3A1C02FBD99C"), "Macon", "Roger", "roger@aol.org", 42),
            new DummyModel(new Guid("5C7164EE-ACCD-828B-BA63-622E3B72E11D"), "Mercedes", "Martinez", "nullam.scelerisque@protonmail.net", 42),
            new DummyModel(new Guid("E0C463C0-176D-154C-5E27-4B5EA485A22E"), "Velma", "Gutierrez", "ullamcorper@aol.couk", 42),
            new DummyModel(new Guid("A34629FF-3F15-4B1D-1C67-15401E18EC4F"), "Brittany", "Mccoy", "et.rutrum@yahoo.net", 42)
        });

        Assert.Equal(6, inserted);

        var count = db.Count<DummyModel>();
        Assert.Equal(12, count);
    }

    [Fact]
    public void Insert_One()
    {
        using var db = GetFakeDatabase();

        var inserted = db.Insert(new DummyModel(new Guid("E762DDCE-4DEE-0F25-751D-F9019DC3462A"), "Brendan", "Zamora", "augue.malesuada@aol.org", 42));

        Assert.True(inserted);

        var count = db.Count<DummyModel>();
        Assert.Equal(7, count);
    }

    [Fact]
    public void Max()
    {
        using var db = GetFakeDatabase();

        var max = db.Max<DummyModel, int>(x => x.Age);
        Assert.Equal(67, max);
    }

    [Fact]
    public void Min()
    {
        using var db = GetFakeDatabase();

        var min = db.Min<DummyModel, int>(x => x.Age);
        Assert.Equal(19, min);
    }

    [Fact]
    public void Update_Single()
    {
        using var db = GetFakeDatabase();
        var found = db.FindOne<DummyModel>(x => x.Lastname == "Brendan");

        found = found with { Lastname = "Brenda"};
        db.Update(found);

        found = db.FindById<DummyModel>(new Guid("E762DDCE-4DEE-0F25-751D-F9019DC3462C"));

        Assert.Equal("Brenda", found.Lastname);
        Assert.Equal("Zamora", found.Firstname);
    }

    [Fact]
    public void Update_Many()
    {
        using var db = GetFakeDatabase();

        db.Update(new List<DummyModel>
        {
            new DummyModel(new Guid("E762DDCE-4DEE-0F25-751D-F9019DC3462C"), "Brenda", "Zamora", "augue.malesuada@aol.org", 21),
            new DummyModel(new Guid("5683855E-B07A-0582-C577-3A1C02FBD991"), "Maron", "Wright", "gravida@aol.org", 42),
        });

        var found = db.FindById<DummyModel>(new Guid("E762DDCE-4DEE-0F25-751D-F9019DC3462C"));

        Assert.Equal("Brenda", found.Lastname);
        Assert.Equal("Zamora", found.Firstname);

        found = db.FindById<DummyModel>(new Guid("5683855E-B07A-0582-C577-3A1C02FBD991"));

        Assert.Equal("Maron", found.Lastname);
        Assert.Equal("Wright", found.Firstname);
    }

    [Fact]
    public void Upsert_Single_Existing()
    {
        using var db = GetFakeDatabase();

        db.Update(new DummyModel(new Guid("E762DDCE-4DEE-0F25-751D-F9019DC3462C"), "Brenda", "Zamora", "augue.malesuada@aol.org", 21));

        var found = db.FindById<DummyModel>(new Guid("E762DDCE-4DEE-0F25-751D-F9019DC3462C"));

        Assert.Equal("Brenda", found.Lastname);
        Assert.Equal("Zamora", found.Firstname);
    }

    [Fact]
    public void Upsert_Single_New()
    {
        using var db = GetFakeDatabase();

        db.Update(new DummyModel(new Guid("E762DDCE-4DEE-0F25-751D-F9019DC34642"), "Brendon", "Zamora", "augue.malesuada@aol.org", 21));

        var found = db.FindById<DummyModel>(new Guid("E762DDCE-4DEE-0F25-751D-F9019DC34642"));

        Assert.Equal("Brendon", found.Lastname);
        Assert.Equal("Zamora", found.Firstname);

        var count = db.Count<DummyModel>();
        Assert.Equal(7, count);
    }

    [Fact]
    public void Upsert_Many()
    {
        using var db = GetFakeDatabase();

        db.Update(new List<DummyModel>
        {
            new DummyModel(new Guid("E762DDCE-4DEE-0F25-751D-F9019DC3462C"), "Brenda", "Zamora", "augue.malesuada@aol.org", 21),
            new DummyModel(new Guid("5683855E-B07A-0582-C577-3A1C02FBD991"), "Maron", "Wright", "gravida@aol.org", 42),
            new DummyModel(new Guid("5683855E-B07A-0582-C577-3A1C02FBD942"), "Billy", "Wright", "gravida@aol.org", 42),
        });

        var found = db.FindById<DummyModel>(new Guid("E762DDCE-4DEE-0F25-751D-F9019DC3462C"));

        Assert.Equal("Brenda", found.Lastname);
        Assert.Equal("Zamora", found.Firstname);

        found = db.FindById<DummyModel>(new Guid("5683855E-B07A-0582-C577-3A1C02FBD991"));

        Assert.Equal("Maron", found.Lastname);
        Assert.Equal("Wright", found.Firstname);

        found = db.FindById<DummyModel>(new Guid("5683855E-B07A-0582-C577-3A1C02FBD942"));

        Assert.Equal("Billy", found.Lastname);
        Assert.Equal("Wright", found.Firstname);

        var count = db.Count<DummyModel>();
        Assert.Equal(7, count);
    }

    private Database GetFakeDatabase()
    {
        var data = new MemoryStream();
        var db = new LiteDatabase(data);

        var collection = db.GetCollection<DummyModel>();

        collection.InsertBulk(new List<DummyModel>
        {
            new DummyModel(new Guid("E762DDCE-4DEE-0F25-751D-F9019DC3462C"), "Brendan", "Zamora", "augue.malesuada@aol.org", 21),
            new DummyModel(new Guid("5683855E-B07A-0582-C577-3A1C02FBD991"), "Macon", "Wright", "gravida@aol.org", 42),
            new DummyModel(new Guid("5683855E-B07A-0582-C577-3A1C02FBD992"), "Macon", "Roger", "roger@aol.org", 51),
            new DummyModel(new Guid("5C7164EE-ACCD-828B-BA63-622E3B72E11E"), "Mercedes", "Martinez", "nullam.scelerisque@protonmail.net", 67),
            new DummyModel(new Guid("E0C463C0-176D-154C-5E27-4B5EA485A22A"), "Velma", "Gutierrez", "ullamcorper@aol.couk", 19),
            new DummyModel(new Guid("A34629FF-3F15-4B1D-1C67-15401E18EC4D"), "Brittany", "Mccoy", "et.rutrum@yahoo.net", 27)
        });

        return new Database(db);
    }
}