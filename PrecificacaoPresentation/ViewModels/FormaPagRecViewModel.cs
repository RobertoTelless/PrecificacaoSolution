using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class FormaPagRecViewModel
    {
        [Key]
        public int FOPR_CD_ID { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }
        public int COBA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE FORMA obrigatorio")]
        public int FOPA_IN_TIPO_FORMA { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve ter no minimo 1 caractere e no máximo 50.")]
        public string FOPR_NM_NOME_FORMA { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE CADASTRO Deve ser uma data válida")]
        public System.DateTime FOPR_DT_CADASTRO { get; set; }
        public int FOPR_IN_ATIVO { get; set; }
        public int ASSI_CD_ID { get; set; }

        public virtual CONTA_BANCO CONTA_BANCO { get; set; }
        public virtual EMPRESA EMPRESA { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }
    }
}