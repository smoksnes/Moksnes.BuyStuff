﻿using System.Threading.Tasks;

namespace Moksnes.BuyStuff.GrainInterfaces
{
    public interface IHello : Orleans.IGrainWithIntegerKey
    {
        Task<string> SayHello(string greeting);
    }

    public interface IPost : Orleans.IGrainWithGuidKey
    {
        Task AddTitle(string title);
    }
}