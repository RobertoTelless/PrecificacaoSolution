//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace EntitiesServices.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class CUSTO_FIXO
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CUSTO_FIXO()
        {
            this.CONTA_PAGAR = new HashSet<CONTA_PAGAR>();
        }
    
        public int CUFX_CD_ID { get; set; }
        public int EMPR_CD_ID { get; set; }
        public int CACF_CD_ID { get; set; }
        public string CUFX_NM_NOME { get; set; }
        public Nullable<System.DateTime> CUFX_DT_INICIO { get; set; }
        public Nullable<System.DateTime> CUFX_DT_TERMINO { get; set; }
        public Nullable<int> CUFX_IN_NUMERO_OCORRENCIAS { get; set; }
        public int PETA_CD_ID { get; set; }
        public Nullable<int> CUFX_NR_DIA_VENCIMENTO { get; set; }
        public Nullable<decimal> CUFX_VL_VALOR { get; set; }
        public Nullable<System.DateTime> CUFX_DT_CADASTRO { get; set; }
        public Nullable<int> CUFX_IN_ATIVO { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> FORN_CD_ID { get; set; }
        public Nullable<int> CECU_CD_ID { get; set; }
        public Nullable<int> CUFX_IN_TIPO_VALOR { get; set; }
    
        public virtual CATEGORIA_CUSTO_FIXO CATEGORIA_CUSTO_FIXO { get; set; }
        public virtual EMPRESA EMPRESA { get; set; }
        public virtual PERIODICIDADE_TAREFA PERIODICIDADE_TAREFA { get; set; }
        public virtual ASSINANTE ASSINANTE { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CONTA_PAGAR> CONTA_PAGAR { get; set; }
        public virtual FORNECEDOR FORNECEDOR { get; set; }
        public virtual PLANO_CONTA PLANO_CONTA { get; set; }
    }
}
