using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HeedeMoestrup.Models
{
    //public class MyEntity
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}

    public class PageHeader
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string PageHeaderText { get; set; }
        public SelectedView SelectedView { get; set; }
    }
    public enum SelectedView
    {
        Forside = 1, Kunstnere = 2, Nyheder = 3, Udgivelser = 4, Kunst = 5, Kunstforeninger = 6
    }
    public class Artist
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public bool Featured { get; set; }
        public bool HMPublication { get; set; }

        public virtual ICollection<Painting> Paintings { get; set; }

        public virtual ICollection<FilePath> FilePaths { get; set; }
    }
    public class Painting
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Desc { get; set; }
        public string Height { get; set; }
        public string Width { get; set; }
        public int Price { get; set; }
        public string Materials { get; set; }
        [DataType(DataType.Date)]
        public DateTime Age { get; set; }
        public bool HMPublication { get; set; }
        public virtual ICollection<FilePath> FilePaths { get; set; }
        public int PaintingCatId { get; set; }
        public virtual PaintingCat PaintingCat { get; set; }

        // Having a property called <entity>Id defines a relationship
        public int ArtistId { get; set; }
        public virtual Artist Artist { get; set; }
    }
    public class PaintingCat
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Painting> Paintings { get; set; }

    }
    public class NewsArticle
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string SubTitle { get; set; }
        public DateTime Date { get; set; }
        public string Author { get; set; }
        public string BodyText { get; set; }
        public virtual ICollection<FilePath> FilePaths { get; set; }

    }

    //FileUpload tabeller
    public enum FileType
    {
        ArtistAvatar = 1, ItemPhoto = 2, NewsPhoto = 3
    }

    public class FilePath
    {
        [Key]
        public int Id { get; set; }
        [StringLength(255)]
        public string FileName { get; set; }
        public FileType FileType { get; set; }
        public virtual Artist Artist { get; set; }
        public virtual Painting Painting { get; set; }
        public virtual NewsArticle NewsArticle { get; set; }
    }
}