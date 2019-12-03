using System.Threading.Tasks;
using Moksnes.BuyStuff.GrainInterfaces;
using Orleans.EventSourcing;
using Orleans.Providers;

namespace Moksnes.BuyStuff.Grains
{
    [StorageProvider(ProviderName = "OrleansLocalStorage")]
    [LogConsistencyProvider(ProviderName = "LogStorage")]
    public class PostGrain : JournaledGrain<PostState, IPostEvent>, IPost
    {
        async Task IPost.AddTitle(string title)
        {
            RaiseEvent(new PostTitleAdded
            {
                Title = title
            });
            await ConfirmEvents();
        }
    }

    public interface IPostEvent
    {

    }

    public class PostTitleAdded : IPostEvent
    {
        public string Title { get; set; }
    }

    public class PostState 
    {
        public string Name { get; private set; }

        void Apply(PostTitleAdded @event)
        {
            // code that updates the state
            Name = @event.Title;
        }
        //Apply(E2 @event)
        //{
        //    // code that updates the state
        //}

        public PostState()
        {
        }
    }
}