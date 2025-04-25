namespace GameHubApi
{
    /*
     * export interface Game {
  id: number;
  name: string;
  background_image: string;
  rating: number;
  parent_platforms: { platform: Platform} []
  metacritic: number | null;
  rating_top: number; // 1, 2, 3, 4, or 5
}
     */
    public class Game
    {
        public int id { get; set; }

        public string name { get; set; }

        public string background_image { get; set; }

        public int rating { get; set; }

        public int? metacritic { get; set; }

        public int rating_top { get; set; } // 1, 2, 3, 4, or 5
    }
}
