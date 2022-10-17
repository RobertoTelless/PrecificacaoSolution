using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface ITipoEmbalagemAppService : IAppServiceBase<TIPO_EMBALAGEM>
    {
        Int32 ValidateCreate(TIPO_EMBALAGEM item, USUARIO usuario);
        Int32 ValidateEdit(TIPO_EMBALAGEM item, TIPO_EMBALAGEM itemAntes, USUARIO usuario);
        Int32 ValidateDelete(TIPO_EMBALAGEM item, USUARIO usuario);
        Int32 ValidateReativar(TIPO_EMBALAGEM item, USUARIO usuario);

        TIPO_EMBALAGEM CheckExist(TIPO_EMBALAGEM conta, Int32 idAss);
        List<TIPO_EMBALAGEM> GetAllItens(Int32 idAss);
        TIPO_EMBALAGEM GetItemById(Int32 id);
        List<TIPO_EMBALAGEM> GetAllItensAdm(Int32 idAss);
    }
}
