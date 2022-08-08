using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class ClienteViewModel
    {
        [Key]
        public int CLIE_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int USUA_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo TIPO DE PESSOA obrigatorio")]
        public int TIPE_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo CATEGORIA obrigatorio")]
        public int CACL_CD_ID { get; set; }
        public System.DateTime CLIE_DT_CRIACAO { get; set; }
        public int CLIE_IN_ATIVO { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "O NOME deve conter no minimo 1 e no máximo 50 caracteres.")]
        public string CLIE_NM_NOME { get; set; }
        [StringLength(50, ErrorMessage = "A RAZÃO SOCIAL deve conter no máximo 50 caracteres.")]
        public string CLIE_NM_RAZAO_SOCIAL { get; set; }
        public string CLIE_NR_CPF { get; set; }
        public string CLIE_NR_CNPJ { get; set; }
        [StringLength(50, ErrorMessage = "O ENDEREÇO deve conter no máximo 50 caracteres.")]
        public string CLIE_NM_ENDERECO { get; set; }
        [StringLength(10, ErrorMessage = "O NUMERO deve conter no máximo 10 caracteres.")]
        public string CLIE_NR_NUMERO { get; set; }
        [StringLength(20, ErrorMessage = "O COMPLEMENTO deve conter no máximo 20 caracteres.")]
        public string CLIE_NM_COMPLEMENTO { get; set; }
        [StringLength(50, ErrorMessage = "O BAIRRO deve conter no máximo 50 caracteres.")]
        public string CLIE_NM_BAIRRO { get; set; }
        [StringLength(50, ErrorMessage = "A CIDADE deve conter no máximo 50 caracteres.")]
        public string CLIE_NM_CIDADE { get; set; }
        [StringLength(10, ErrorMessage = "O CEP deve conter no máximo 10 caracteres.")]
        public string CLIE_NR_CEP { get; set; }
        [StringLength(20, ErrorMessage = "O TELEFONE deve conter no máximo 20 caracteres.")]
        public string CLIE_NR_TELEFONE { get; set; }
        [StringLength(20, ErrorMessage = "O CELULAR deve conter no máximo 20 caracteres.")]
        public string CLIE_NR_CELULAR { get; set; }
        [StringLength(20, ErrorMessage = "O WHATSAPP deve conter no máximo 20 caracteres.")]
        public string CLIE_NR_WHATSAPP { get; set; }
        [Required(ErrorMessage = "Campo E-MAIL obrigatorio")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "O E-MAIL deve conter no minimo 1 e no máximo 100 caracteres.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string CLIE_NM_EMAIL { get; set; }
        [StringLength(2000, ErrorMessage = "A OBSERVAÇÃO deve conter no máximo 2000 caracteres.")]
        public string CLIE_TX_OBSERVACOES { get; set; }
        public Nullable<int> UF_CD_ID { get; set; }
        [StringLength(20, ErrorMessage = "A INSCRIÇÃO ESTADUAL deve conter no máximo 20 caracteres.")]
        public string CLIE_NR_INSCRICAO_ESTADUAL { get; set; }
        [StringLength(50, ErrorMessage = "A SITUAÇÃO deve conter no máximo 50 caracteres.")]
        public string CLIE_NM_SITUACAO { get; set; }
        public string CLIE_AQ_FOTO { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CATEGORIA_CLIENTE CATEGORIA_CLIENTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE_ANEXO> CLIENTE_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE_ANOTACAO> CLIENTE_ANOTACAO { get; set; }
        public virtual USUARIO USUARIO { get; set; }
        public virtual TIPO_PESSOA TIPO_PESSOA { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DOCUMENTO> DOCUMENTO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE_CONTATO> CLIENTE_CONTATO { get; set; }
        public virtual UF UF { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CLIENTE_QUADRO_SOCIETARIO> CLIENTE_QUADRO_SOCIETARIO { get; set; }

    }
}