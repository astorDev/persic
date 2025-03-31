using Shouldly;

namespace Persic.EF.Playground;

[TestClass]
public class SearchShould 
{
    [TestMethod]
    public async Task FindStringIdEntity()
    {
        var context = Given.Empty<PlaygroundContext>();
        var id = "c3a10f1c-ff8d-4e75-af05-a5660228b669";
        context.Add(new PlaygroundEntity { 
            Id = id,
            Name = "FindStringIdEntity" 
        });

        context.SaveChanges();
        context = new PlaygroundContext();
        var found = await context.Entities.Search(id);
        found.ShouldNotBeNull();
        found!.Name.ShouldBe("FindStringIdEntity");
    }

    [TestMethod]
    public async Task FindIntIdEntity()
    {
        var context = Given.Empty<PlaygroundContext>();

        context.Add(new IntIdEntity { 
            Name = "FindIntIdEntity" 
        });

        context.SaveChanges();
        context = new PlaygroundContext();
        var found = await context.IntIdEntities.Search(1);
        found.ShouldNotBeNull();
        found!.Name.ShouldBe("FindIntIdEntity");
    }
}