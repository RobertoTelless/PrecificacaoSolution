using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using EntitiesServices.Model;

namespace ERP_Condominios_Solution.ViewModels
{
    public class PessoaExternaViewModel
    {
        [Key]
        public int PEEX_CD_ID { get; set; }
        public int ASSI_CD_ID { get; set; }
        public int CARG_CD_ID { get; set; }
        [Required(ErrorMessage = "Campo NOME obrigatorio")]
        public string PEEX_NM_NOME { get; set; }
        [Required(ErrorMessage = "Campo CPF obrigatorio")]
        [StringLength(20, ErrorMessage = "O CPF deve ter no máximo 20 caracteres.")]
        public string PEEX_NR_CPF { get; set; }
        [StringLength(20, ErrorMessage = "O RG deve ter no máximo 20 caracteres.")]
        public string PEEX_NR_RG { get; set; }
        [StringLength(150, ErrorMessage = "O E-MAIL deve ter no máximo 150 caracteres.")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,6}$", ErrorMessage = "Deve ser um e-mail válido")]
        public string PEES_EM_EMAIL { get; set; }
        [StringLength(20, ErrorMessage = "O TELEFONE deve ter no máximo 20 caracteres.")]
        public string PEEX_NR_TELEFONE { get; set; }
        [StringLength(20, ErrorMessage = "O CELULAR deve ter no máximo 20 caracteres.")]
        public string PEEX_NR_CELULAR { get; set; }
        [DataType(DataType.Date, ErrorMessage = "DATA DE CADASTRO Deve ser uma data válida")]
        public System.DateTime PEEX_DT_CADASTRO { get; set; }
        public int PEEX_IN_ATIVO { get; set; }

        public virtual ASSINANTE ASSINANTE { get; set; }
        public virtual CARGO_USUARIO CARGO_USUARIO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PESSOA_EXTERNA_ANEXO> PESSOA_EXTERNA_ANEXO { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PESSOA_EXTERNA_ANOTACAO> PESSOA_EXTERNA_ANOTACAO { get; set; }
    }
}