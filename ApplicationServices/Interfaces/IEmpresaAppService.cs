using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;

namespace ApplicationServices.Interfaces
{
    public interface IEmpresaAppService : IAppServiceBase<EMPRESA>
    {
        Int32 ValidateCreate(EMPRESA item, USUARIO usuario);
        Int32 ValidateEdit(EMPRESA item, EMPRESA itemAntes, USUARIO usuario);
        Int32 ValidateEdit(EMPRESA item, EMPRESA itemAntes);
        Int32 ValidateDelete(EMPRESA item, USUARIO usuario);
        Int32 ValidateReativar(EMPRESA item, USUARIO usuario);

        EMPRESA CheckExist(EMPRESA item, Int32 idAss);
        EMPRESA GetItemById(Int32 id);
        List<EMPRESA> GetAllItens(Int32 idAss);
        List<EMPRESA> GetAllItensAdm(Int32 idAss);
        Int32 ExecuteFilter(String nome, Int32 idAss, out List<EMPRESA> objeto);

        List<MAQUINA> GetAllMaquinas(Int32 idAss);
        List<REGIME_TRIBUTARIO> GetAllRegimes();
        EMPRESA_ANEXO GetAnexoById(Int32 id);

        EMPRESA_MAQUINA GetMaquinaById(Int32 id);
        Int32 ValidateEditMaquina(EMPRESA_MAQUINA item);
        Int32 ValidateCreateMaquina(EMPRESA_MAQUINA item);
        EMPRESA_MAQUINA GetByEmpresaMaquina(Int32 empresa, Int32 maquina);
    }
}
