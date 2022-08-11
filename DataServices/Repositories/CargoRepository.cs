using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using EntitiesServices.Work_Classes;
using System.Data.Entity;

namespace DataServices.Repositories
{
    public class CargoRepository : RepositoryBase<CARGO_USUARIO>, ICargoRepository
    {
        public CARGO_USUARIO CheckExist(CARGO_USUARIO conta, Int32 idAss)
        {
            IQueryable<CARGO_USUARIO> query = Db.CARGO_USUARIO;
            query = query.Where(p => p.CARG_NM_NOME == conta.CARG_NM_NOME);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public CARGO_USUARIO GetItemById(Int32 id)
        {
            IQueryable<CARGO_USUARIO> query = Db.CARGO_USUARIO;
            query = query.Where(p => p.CARG_CD_ID == id);
            return query.FirstOrDefault();
        }

        public List<CARGO_USUARIO> GetAllItensAdm(Int32 idAss)
        {
            IQueryable<CARGO_USUARIO> query = Db.CARGO_USUARIO;
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

        public List<CARGO_USUARIO> GetAllItens(Int32 idAss)
        {
            IQueryable<CARGO_USUARIO> query = Db.CARGO_USUARIO.Where(p => p.CARG_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.ToList();
        }

    }
}
 