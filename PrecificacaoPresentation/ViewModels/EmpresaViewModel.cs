using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Attributes;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class EmpresaViewModel
    {
        [Key]
        public int EMPR_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int RETR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME DA EMPRESA obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O NOME DA EMPRESA deve conter no minimo 1 caracteres e no máximo 100 caracteres.")]
        public string EMPR_NM_NOME { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_VL_PATRIMONIO_LIQUIDO { get; set; }
        public string PatrimonioText
        {
            get
            {
                return EMPR_VL_PATRIMONIO_LIQUIDO.HasValue ? CrossCutting.Formatters.DecimalFormatter(EMPR_VL_PATRIMONIO_LIQUIDO.Value) : string.Empty;
            }
            set
            {
                EMPR_VL_PATRIMONIO_LIQUIDO = Convert.ToDecimal(CrossCutting.CommonHelpers.GetOnlyDigits(value, true));
            }
        }
        [DataType(DataType.Date, ErrorMessage = "DATA DE CADASTRO Deve ser uma data válida")]
        public System.DateTime EMPR_DT_CADASTRO { get; set; }
        public int EMPR_IN_ATIVO { get; set; }
        public Nullable<int> MAQN_CD_ID { get; set; }
        public int EMPR_IN_OPERA_CARTAO { get; set; }
        public string EMPR_NM_OUTRA_MAQUINA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_PC_ANTECIPACAO { get; set; }
        public int EMPR_IN_PAGA_COMISSAO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_VL_IMPOSTO_MEI { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_PC_VENDA_DEBITO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_PC_VENDA_CREDITO { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_PC_VENDA_DINHEIRO { get; set; }
        [StringLength(100, ErrorMessage = "A RAZÃO SOCIAL deve conter no máximo 100 caracteres.")]
        public string EMPR_NM_RAZAO { get; set; }
        [StringLength(20, ErrorMessage = "O CNPJ deve conter no máximo 20 caracteres.")]
        [CustomValidationCNPJ(ErrorMessage = "CNPJ inválido")]
        [Required(ErrorMessage = "Campo CNPJ obrigatorio")]
        public string EMPR_NR_CNPJ { get; set; }
        [StringLength(20, ErrorMessage = "A INSCRIÇÃO MUNICIPAL deve conter no máximo 20 caracteres.")]
        public string EMPR_NR_INSCRICAO_MUNICIPAL { get; set; }
        [StringLength(20, ErrorMessage = "A INSCRIÇÃO ESTADUAL deve conter no máximo 20 caracteres.")]
        public string EMPR_NR_INSCRICAO_ESTADUAL { get; set; }
        [StringLength(50, ErrorMessage = "O ENDEREÇO deve conter no máximo 50 caracteres.")]
        public string EMPR_NM_ENDERECO { get; set; }
        [StringLength(20, ErrorMessage = "O NÚMERO deve conter no máximo 20 caracteres.")]
        public string EMPR_NM_NUMERO { get; set; }
        [StringLength(20, ErrorMessage = "O COMPLEMENTO deve conter no máximo 20 caracteres.")]
        public string EMPR_NM_COMPLEMENTO { get; set; }
        [StringLength(50, ErrorMessage = "O BAIRRO deve conter no máximo 50 caracteres.")]
        public string EMPR_NM_BAIRRO { get; set; }
        [StringLength(50, ErrorMessage = "A CIDADE deve conter no máximo 50 caracteres.")]
        public string EMPR_NM_CIDADE { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        [StringLength(10, ErrorMessage = "O CEP deve conter no máximo 10 caracteres.")]
        public string EMPR_NR_CEP { get; set; }
        public Nullable<int> PLEN_CD_ID { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_VL_TAXA_MEDIA { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_VL_COMISSAO_VENDEDOR { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_VL_COMISSAO_OUTROS { get; set; }
        [RegularExpression(@"^[0-9]+([,.][0-9]+)?$", ErrorMessage = "Deve ser um valor numérico positivo")]
        public Nullable<decimal> EMPR_VL_TAXA_MEDIA_DEBITO { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE> CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CUSTO_FIXO> CUSTO_FIXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPRESA_ANEXO> EMPRESA_ANEXO { get; set; }
        public virtual MAQUINA MAQUINA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPRESA_MAQUINA> EMPRESA_MAQUINA { get; set; }
        public virtual UF UF { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORNECEDOR> FORNECEDOR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MOVIMENTO_ESTOQUE_PRODUTO> MOVIMENTO_ESTOQUE_PRODUTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRANSPORTADORA> TRANSPORTADORA { get; set; }
        public virtual REGIME_TRIBUTARIO REGIME_TRIBUTARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORMA_PAGTO_RECTO> FORMA_PAGTO_RECTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_ESTOQUE_EMPRESA> PRODUTO_ESTOQUE_EMPRESA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_TABELA_PRECO> PRODUTO_TABELA_PRECO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPRESA_CUSTO_VARIAVEL> EMPRESA_CUSTO_VARIAVEL { get; set; }
        public virtual PLATAFORMA_ENTREGA PLATAFORMA_ENTREGA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EMPRESA_PLATAFORMA> EMPRESA_PLATAFORMA { get; set; }
    }
}