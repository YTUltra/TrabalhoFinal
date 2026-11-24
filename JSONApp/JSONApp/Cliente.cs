using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace JSONApp
{
    public class Cliente
    {
        [JsonPropertyName("nome")]
        public string Nome { get; set; }

        [JsonPropertyName("anoNascimento")]
        public int AnoNascimento { get; set; }

        [JsonPropertyName("cpf")]
        public string CPF { get; set; }
    }
}
