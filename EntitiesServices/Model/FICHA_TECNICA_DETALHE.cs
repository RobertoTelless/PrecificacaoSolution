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
    
    public partial class FICHA_TECNICA_DETALHE
    {
        public int FITD_CD_ID { get; set; }
        public int FITE_CD_ID { get; set; }
        public int PROD_CD_ID { get; set; }
        public Nullable<decimal> FITD_QN_QUANTIDADE { get; set; }
        public Nullable<int> FITD_IN_ATIVO { get; set; }
    
        public virtual PRODUTO PRODUTO { get; set; }
    }
}
