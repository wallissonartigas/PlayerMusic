using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlayerMusic.Models
{
    public class MusicModel
    {
            public int Id { get; set; }
            public string Autor { get; set; }
            public string Genero { get; set; }
            [Display (Name = "Imagem do Álbum")]
            public string AlbumDiretorio { get; set; }
            public string MusicaDiretorio { get; set; }
            public string NomeMusica { get; set; }
        
    }
}
