using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayerMusic.Models
{
    public class MusicModel
    {
            public int Id { get; set; }
            public string Autor { get; set; }
            public string Genero { get; set; }
            public string AlbumDiretorio { get; set; }
            public string MusicaDiretorio { get; set; }
            public string NomeMusica { get; set; }
        
    }
}
