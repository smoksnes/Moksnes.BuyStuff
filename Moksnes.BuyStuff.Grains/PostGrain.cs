using System.Threading.Tasks;
using Moksnes.BuyStuff.GrainInterfaces;
using Orleans.EventSourcing;
using Orleans.Providers;

namespace Moksnes.BuyStuff.Grains
{
    [StorageProvider(ProviderName = "StorageProvider")]
    [LogConsistencyProvider(ProviderName = "LogStorage")]
    public class PostGrain : JournaledGrain<PostState, IPostEvent>, IPost
    {
        Task IPost.AddTitle(string title)
        {
            RaiseEvent(new PostTitleAdded
            {
                Title = title
            });
            return ConfirmEvents();
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
        public string Title { get; private set; }

        public PostState Apply(PostTitleAdded @event)
        {
            // code that updates the state
            Title = @event.Title;
            return this;
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