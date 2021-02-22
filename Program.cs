using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NLog.Web;

namespace DotNetDb_Assignment4
{
    class Program
    {
        private static NLog.Logger logger = NLogBuilder.ConfigureNLog(Directory.GetCurrentDirectory() + "\\nlog.config").GetCurrentClassLogger();
        static void Main(string[] args)
        {
            logger.Info("Program started");
            Console.WriteLine("Welcome to the Movie Lister!");
            getMenu();
        }
        public static void getMenu() {
            string file = "movies.csv";
            MovieList movieFile = new MovieList(file);
            string choice;
            try {
                do {
                    Console.Write("1. List movies from file\n2. Add movie to file\n3. Exit\nUser choice: ");
                    choice = Console.ReadLine();
                    Console.WriteLine();
                    if (choice == "1") {
                        foreach (Movie m in movieFile.movieList) {
                            Console.WriteLine(m.toString());
                        };
                    }
                    else if (choice == "2") {
                        Console.Write("Add movie title: ");
                        string newTitle = Console.ReadLine();
                        int count = 0;
                        foreach (Movie m in movieFile.movieList) {
                            if (m.title == newTitle) {
                                Console.WriteLine("Movie already exists.\n");
                                getMenu();
                            }
                            else {
                                count++;
                            }
                        }
                        var genreList = new List<string>();
                        string anotherGenre;
                        do {
                            Console.Write("Add genre: ");
                            genreList.Add(Console.ReadLine());
                            Console.Write("Add another genre (Y/N): ");
                            anotherGenre = Console.ReadLine().ToUpper();
                        } while (anotherGenre == "Y");
                        string genre = string.Join("|", genreList);
                        StreamWriter sw = new StreamWriter(file, true);
                        sw.WriteLine($"{count+1},{newTitle},{genre}");
                        sw.Close();
                        Console.WriteLine($"{newTitle} added to movie list.\n");
                    }
                    else {
                        Console.WriteLine("Goodbye.");
                        logger.Info("Program ended");
                    }
                } while (choice != "3");
            } catch (Exception ex) {
                logger.Error(ex.Message);
            }
        }
    }
    public class Movie
    {
        public int movieId { get; set; }
        public string title { get; set; }
        public List<string> genres { get; set; }
        public Movie() {
            genres = new List<string>();
        }
        public string toString() {
            return $"MovieID: {movieId} Title: {title} Genre(s): {string.Join(", ", genres)}\n";
        }
    }
    public class MovieList
    {
        public string path { get; set; }
        public List<Movie> movieList { get; set; }
        public MovieList(string path) {
            movieList = new List<Movie>();
            this.path = path;
            StreamReader sr = new StreamReader(path);
            sr.ReadLine();
            while (!sr.EndOfStream) {
                Movie movie = new Movie();
                string line = sr.ReadLine();
                int lineIndex = line.IndexOf('"');
                if (lineIndex == -1) {
                    string[] movieSplit = line.Split(',');
                    movie.movieId = Int32.Parse(movieSplit[0]);
                    movie.title = movieSplit[1];
                    movie.genres = movieSplit[2].Split('|').ToList();
                }
                else {
                    movie.movieId = Int32.Parse(line.Substring(0, lineIndex - 1));
                    line = line.Substring(lineIndex + 1);
                    lineIndex = line.IndexOf('"');
                    movie.title = line.Substring(0, lineIndex);
                    line = line.Substring(lineIndex + 2);
                    movie.genres = line.Split('|').ToList();
                }
                movieList.Add(movie);
            }
            sr.Close();
        }
    }
}
