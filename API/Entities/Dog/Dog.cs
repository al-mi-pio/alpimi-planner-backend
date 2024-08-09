namespace AlpimiAPI.Dog
{
    public class Dog
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public int BreedId { get; set; }
        public AlpimiAPI.Breed.Breed Breed { get; set; }
    }
}

