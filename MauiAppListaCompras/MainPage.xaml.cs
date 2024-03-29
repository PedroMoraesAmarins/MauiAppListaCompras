﻿using System.Collections.ObjectModel;
using MauiAppListaCompras.Models;

namespace MauiAppListaCompras
{
    public partial class MainPage : ContentPage
    {   
        ObservableCollection<Produto> lista_produtos = new ObservableCollection<Produto>();

        public MainPage()
        {
            InitializeComponent();
            lst_produtos.ItemsSource = lista_produtos;
        }

        private void ToolbarItem_Clicked_Somar(System.Object sender, System.EventArgs e)
        {
            double soma = lista_produtos.Sum(i => (i.Preco * i.Quantidade));
            string msg = $"O total é {soma:C}";
            DisplayAlert("Somatória", msg,"Fechar");
        }

        protected async override void OnAppearing()
        {
            if(lista_produtos.Count == 0) 
            {
                    List<Produto> tmp = await App.Db.GetAll();
                    foreach (Produto p in tmp) 
                    { 
                        lista_produtos.Add(p);
                    }
            } // Fecha if
            base.OnAppearing(); 
        }

        private async void ToolbarItem_Clicked_Add(System.Object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new Views.NovoProduto2());
        }

        private async void txt_search_TextChanged(System.Object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
        {
            string q = e.NewTextValue;
            lista_produtos.Clear();

            List<Produto> tmp = await App.Db.Search(q);
            foreach (Produto p in tmp) 
            {
               lista_produtos.Add(p);
            } 
        }

        private void ref_carregando_Refreshing(System.Object sender, System.EventArgs e)
        {
            lista_produtos.Clear();
            Task.Run(async () =>
            {
                List<Produto> tmp = await App.Db.GetAll();
                foreach (Produto p in tmp)
                {
                    lista_produtos.Add(p);
                }
            });
            ref_carregando.IsRefreshing = false;
        }

        private void lst_produtos_ItemSelected(System.Object sender, Microsoft.Maui.Controls.SelectedItemChangedEventArgs e)
        {
            Produto? p = e.SelectedItem as Produto;

            Navigation.PushAsync(new Views.EditarProduto2
            {
                BindingContext = p
            });
        }

        private async void MenuItem_Clicked_Remover(System.Object sender, System.EventArgs e)
        {
            try
            {
                MenuItem selecionado = (MenuItem)sender;

                Produto p = selecionado.BindingContext as Produto;

                bool confirm = await DisplayAlert(
                    "Tem certeza?", "Remover Produto?", "Sim", "Cancelar");

                if (confirm) 
                {
                    await App.Db.Delete(p.Id);
                    await DisplayAlert("Sucesso!", "Produto Removido", "OK");
                    lista_produtos.Remove(p);
                }
            }
            catch (Exception ex) 
            {
                await DisplayAlert("Ops", ex.Message, "OK");
            }
        }
    }
}
