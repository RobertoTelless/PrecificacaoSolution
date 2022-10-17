using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using EntitiesServices.Work_Classes;

namespace ModelServices.Interfaces.EntitiesServices
{
    public interface ITipoEmbalagemService : IServiceBase<TIPO_EMBALAGEM>
    {
        Int32 Create(TIPO_EMBALAGEM perfil, LOG log);
        Int32 Create(TIPO_EMBALAGEM perfil);
        Int32 Edit(TIPO_EMBALAGEM perfil, LOG log);
        Int32 Edit(TIPO_EMBALAGEM perfil);
        Int32 Delete(TIPO_EMBALAGEM perfil, LOG log);

        TIPO_EMBALAGEM CheckExist(TIPO_EMBALAGEM conta, Int32 idAss);
        TIPO_EMBALAGEM GetItemById(Int32 id);
        List<TIPO_EMBALAGEM> GetAllItens(Int32 idAss);
        List<TIPO_EMBALAGEM> GetAllItensAdm(Int32 idAss);
    }
}
