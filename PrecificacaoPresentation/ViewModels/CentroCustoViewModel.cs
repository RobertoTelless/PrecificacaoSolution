using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class CentroCustoViewModel
    {
        [Key]
        public int CECU_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo GRUPO obrigatorio")]
        public int GRCC_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo SUBGRUPO obrigatorio")]
        public int SGCC_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NÙMERO obrigatorio")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "O NÙMERO deve conter no minimo 1 e no máximo 10 caracteres.")]
        public string CECU_NR_NUMERO { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 100 caracteres.")]
        public string CECU_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE CONTA obrigatorio")]
        public int CECU_IN_TIPO { get; set; }
        public int CECU_IN_MOVTO { get; set; }
        public int CECU_IN_ATIVO { get; set; }
        public string CECU_NM_EXIBE { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_PAGAR> CONTA_PAGAR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_PAGAR_RATEIO> CONTA_PAGAR_RATEIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_RECEBER> CONTA_RECEBER { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_RECEBER_RATEIO> CONTA_RECEBER_RATEIO { get; set; }
        public virtual GRUPO_PLANO_CONTA GRUPO_PLANO_CONTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PEDIDO_COMPRA> PEDIDO_COMPRA { get; set; }
        public virtual SUBGRUPO_PLANO_CONTA SUBGRUPO_PLANO_CONTA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CUSTO_FIXO> CUSTO_FIXO { get; set; }
    }
}