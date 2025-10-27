﻿using MovieStoreApi.Models;

namespace MoviesStore.models
{
    public class Favorite
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public Movie Movie { get; set; }= null!;


        public int ProfileId { get; set; }
        public Profile Profile { get; set; } = null!;


    }
}
