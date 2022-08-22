using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class SubgrupoViewModel
    {
        [Key]
        public int SGCC_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo GRUPO obrigatorio")]
        public int GRCC_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NÙMERO obrigatorio")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "O NÙMERO deve conter no minimo 1 e no máximo 10 caracteres.")]
        public string SGCC_NR_NUMERO { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 100 caracteres.")]
        public string SGCC_NM_NOME { get; set; }
        [StringLength(500, ErrorMessage = "A DESCRIÇÃO deve conter no máximo 500 caracteres.")]
        public string SGCC_DS_DESCRICAO { get; set; }
        public Nullable<int> SGCC_IN_ATIVO { get; set; }
        public string SGCC_NM_EXIBE { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual GRUPO_PLANO_CONTA GRUPO_PLANO_CONTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PLANO_CONTA> PLANO_CONTA { get; set; }

    }
}