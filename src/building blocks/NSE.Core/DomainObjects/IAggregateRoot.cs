namespace NSE.Core.DomainObjects
{
    // IAggregateRoot interface de marcação usada apenas para indicar que essa classe de entidade também é uma raiz de agregação
    // Uma raiz de agregação é responsável por gerenciar os objetos de domínio filhos e garantir que os dados 
    // serão sempre salvos em conjunto, sendo assim, o repositório terá acesso somente a raiz de agregação e não aos objetos filhos
    public interface IAggregateRoot { }
}
