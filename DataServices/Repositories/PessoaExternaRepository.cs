using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntitiesServices.Model;
using ModelServices.Interfaces.Repositories;
using System.Data.Entity;
using EntitiesServices.Work_Classes;

namespace DataServices.Repositories
{
    public class PessoaExternaRepository : RepositoryBase<PESSOA_EXTERNA>, IPessoaExternaRepository
    {
        public PESSOA_EXTERNA CheckExist(PESSOA_EXTERNA tarefa, Int32 idUsu)
        {
            IQueryable<PESSOA_EXTERNA> query = Db.PESSOA_EXTERNA;
            query = query.Where(p => p.PEEX_NR_CPF == tarefa.PEEX_NR_CPF);
            query = query.Where(p => p.ASSI_CD_ID == tarefa.ASSI_CD_ID);
            return query.FirstOrDefault();
        }

        public PESSOA_EXTERNA GetItemById(Int32 id)
        {
            IQueryable<PESSOA_EXTERNA> query = Db.PESSOA_EXTERNA;
            query = query.Where(p => p.PEEX_CD_ID == id);
            return query.FirstOrDefault();
        }

        public PESSOA_EXTERNA GetByEmail(String email, Int32 idAss)
        {
            IQueryable<PESSOA_EXTERNA> query = Db.PESSOA_EXTERNA;
            query = query.Where(p => p.PEES_EM_EMAIL == email);
            query = query.Where(p => p.ASSI_CD_ID == idAss);
            return query.FirstOrDefault();
        }

        public List<PESSOA_EXTERNA> GetAllItens(Int32 idUsu)
        {
            IQueryable<PESSOA_EXTERNA> query = Db.PESSOA_EXTERNA.Where(p => p.PEEX_IN_ATIVO == 1);
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            return query.ToList();
        }

        public List<PESSOA_EXTERNA> GetAllItensAdm(Int32 idUsu)
        {
            IQueryable<PESSOA_EXTERNA> query = Db.PESSOA_EXTERNA;
            query = query.Where(p => p.ASSI_CD_ID == idUsu);
            return query.ToList();
        }

        public List<PESSOA_EXTERNA> ExecuteFilter(Int32? cargo, String nome, String cpf, String email, Int32 idAss)
        {
            List<PESSOA_EXTERNA> lista = new List<PESSOA_EXTERNA>();
            IQueryable<PESSOA_EXTERNA> query = Db.PESSOA_EXTERNA;
            if (!String.IsNullOrEmpty(nome))
            {
                query = query.Where(p => p.PEEX_NM_NOME.Contains(nome));
            }
            if (!String.IsNullOrEmpty(cpf))
            {
                query = query.Where(p => p.PEEX_NR_CPF == cpf);
            }
            if (!String.IsNullOrEmpty(email))
            {
                query = query.Where(p => p.PEES_EM_EMAIL.Contains(email));
            }
            if (cargo > 0)
            {
                query = query.Where(p => p.CARG_CD_ID == cargo);
            }
            if (query != null)
            {
                query = query.OrderBy(a => a.PEEX_NM_NOME);
                query = query.Where(p => p.ASSI_CD_ID == idAss);
                lista = query.ToList<PESSOA_EXTERNA>();
            }
            return lista;
        }
    }
}
 