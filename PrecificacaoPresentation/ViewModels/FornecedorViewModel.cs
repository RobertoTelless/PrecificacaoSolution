using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;
using EntitiesServices.Attributes;

namespace ERP_Condominios_Solution.ViewModels
{
    public class FornecedorViewModel
    {
        [Key]
        public int FORN_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public Nullable<int> EMPR_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CATEGORIA obrigatorio")]
        public int CAFO_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE PESSOA obrigatorio")]
        public int TIPE_CD_ID { get; set; }
        [StringLength(250, ErrorMessage = "O NOME DO LOGO deve conter no máximo 250 caracteres.")]
        public string FORN_AQ_LOGOTIPO { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 100 caracteres.")]
        public string FORN_NM_NOME { get; set; }
        [StringLength(100, ErrorMessage = "A RAZÃO SOCIAL deve conter no máximo 100 caracteres.")]
        public string FORN_NM_RAZAO_SOCIAL { get; set; }
        [StringLength(20, ErrorMessage = "O CPF deve conter no máximo 20 caracteres.")]
        [CustomValidationCPF(ErrorMessage = "CPF inválido")]
        public string FORN_NR_CPF { get; set; }
        [StringLength(20, ErrorMessage = "O CNPJ deve conter no máximo 20 caracteres.")]
        [CustomValidationCNPJ(ErrorMessage = "CNPJ inválido")]
        public string FORN_NR_CNPJ { get; set; }
        [StringLength(20, ErrorMessage = "O TELEFONE deve conter no máximo 20 caracteres.")]
        public string FORN_NR_TELEFONE { get; set; }
        [StringLength(20, ErrorMessage = "O CELULAR deve conter no máximo 20 caracteres.")]
        public string FORN_NR_CELULAR { get; set; }
        [StringLength(20, ErrorMessage = "O WHATSAPP deve conter no máximo 20 caracteres.")]
        public string FORN_NR_WHATSAPP { get; set; }
        [StringLength(100, ErrorMessage = "O E-MAIL deve conter no máximo 100 caracteres.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string FORN_EM_EMAIL { get; set; }
        [StringLength(200, ErrorMessage = "O ENDEREÇO deve conter no máximo 200 caracteres.")]
        public string FORN_NM_ENDERECO { get; set; }
        [StringLength(50, ErrorMessage = "O BAIRRO deve conter no máximo 50 caracteres.")]
        public string FORN_NM_BAIRRO { get; set; }
        [StringLength(50, ErrorMessage = "A CIDADE deve conter no máximo 50 caracteres.")]
        public string FORN_NM_CIDADE { get; set; }
        [StringLength(15, ErrorMessage = "O CEP deve conter no máximo 15 caracteres.")]
        public string FORN_NR_CEP { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        [StringLength(250, ErrorMessage = "A SITUAÇÃO deve conter no máximo 250 caracteres.")]
        public string FORN_DS_SITUACAO { get; set; }
        public byte[] FORN_TX_OBSERVACOES { get; set; }
        public System.DateTime FORN_DT_CADASTRO { get; set; }
        public int FORN_IN_ATIVO { get; set; }
        [StringLength(5000, ErrorMessage = "A OBSERVAÇÃO deve conter no máximo 5000 caracteres.")]
        public string FORN_TX_OBSERVACAO { get; set; }
        [StringLength(50, ErrorMessage = "A INSCRIÇÃO ESTADUAL deve conter no máximo 50 caracteres.")]
        public string FORN_NR_INSCRICAO_ESTADUAL { get; set; }
        [StringLength(50, ErrorMessage = "A INSCRIÇÃO MUNICIPAL deve conter no máximo 50 caracteres.")]
        public string FORN_NR_INSCRICAO_MUNICIPAL { get; set; }
        [StringLength(250, ErrorMessage = "O WEBSITE deve conter no máximo 250 caracteres.")]
        public string FORN_LK_WEBSITE { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CATEGORIA_FORNECEDOR CATEGORIA_FORNECEDOR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_PAGAR> CONTA_PAGAR { get; set; }
        public virtual EMPRESA EMPRESA { get; set; }
        public virtual TIPO_PESSOA TIPO_PESSOA { get; set; }
        public virtual UF UF { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ITEM_PEDIDO_COMPRA> ITEM_PEDIDO_COMPRA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PEDIDO_COMPRA> PEDIDO_COMPRA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORNECEDOR_ANOTACOES> FORNECEDOR_ANOTACOES { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORNECEDOR_QUADRO_SOCIETARIO> FORNECEDOR_QUADRO_SOCIETARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORNECEDOR_CONTATO> FORNECEDOR_CONTATO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FORNECEDOR_ANEXO> FORNECEDOR_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PRODUTO_FORNECEDOR> PRODUTO_FORNECEDOR { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CUSTO_FIXO> CUSTO_FIXO { get; set; }

    }
}