using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class FichaTecnicaViewModel
    {
        [Key]
        public int FITE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo PRODUTO obrigatorio")]
        public int PROD_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public int FITE_CD_FICHA_PAI { get; set; }
        [StringLength(250, ErrorMessage = "O NOME DA FOTO deve conter no máximo 250 caracteres.")]
        public string FITE_AQ_FOTO_APRESENTACAO { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 100 caracteres.")]
        public string FITE_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo FORMA DE MONTAGEM obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "A FORMA DE MONTAGEM deve conter no minimo 1 e no máximo 5000 caracteres.")]
        public string FITE_DS_DESCRICAO { get; set; }
        [Required(ErrorMessage = "Campo FORMA DE APRESENTAÇÃO obrigatorio")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "A FORMA DE APRESENTAÇÃO deve conter no minimo 1 e no máximo 5000 caracteres.")]
        public string FITE_DS_APRESENTACAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<int> FITE_NR_PORCOES { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> FITE_NR_PESO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> FITE_NR_FATOR_CORRECAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> FITE_QN_QUANTIDADE_LIQUIDA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> FITE_PC_PERCENTUAL_PERDA { get; set; }
        public Nullable<System.DateTime> FITE_DT_CADASTRO { get; set; }
        public Nullable<int> FITE_IN_ATIVO { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FICHA_TECNICA> FICHA_TECNICA1 { get; set; }
        public virtual FICHA_TECNICA FICHA_TECNICA2 { get; set; }
        public virtual PRODUTO_TABELA_PRECO PRODUTO_TABELA_PRECO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FICHA_TECNICA_DETALHE> FICHA_TECNICA_DETALHE { get; set; }
        public virtual PRODUTO PRODUTO { get; set; }
    }
}