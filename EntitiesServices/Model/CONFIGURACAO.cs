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
    
    public partial class CONFIGURACAO
    {
        public int CONF_CD_ID { get; set; }
        public Nullable<int> ASSI_CD_ID { get; set; }
        public Nullable<int> CONF_NR_FALHAS_DIA { get; set; }
        public string CONF_NM_HOST_SMTP { get; set; }
        public string CONF_NM_PORTA_SMTP { get; set; }
        public string CONF_NM_EMAIL_EMISSOO { get; set; }
        public string CONF_NM_SENHA_EMISSOR { get; set; }
        public Nullable<int> CONF_NR_REFRESH_DASH { get; set; }
        public string CONF_NM_ARQUIVO_ALARME { get; set; }
        public Nullable<int> CONF_NR_REFRESH_NOTIFICACAO { get; set; }
        public string CONF_SG_LOGIN_SMS { get; set; }
        public string CONF_SG_SENHA_SMS { get; set; }
        public Nullable<int> CONF_IN_CNPJ_DUPLICADO { get; set; }
    
        public virtual ASSINANTE ASSINANTE { get; set; }
    }
}
