import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class SharedService {
  readonly APIUrl="https://localhost:7022"

  headersAuth = new HttpHeaders().append("Authorization", `Bearer ${localStorage.getItem('token')}`);

  constructor(private http:HttpClient) { }

  //#region User

  getUserbyToken(token:any):Observable<any[]>{
    return this.http.post<any>(this.APIUrl+'/User/GetUserByToken?token='+token, token)
  }

  login(account:any):Observable<any[]>{
    return this.http.post<any>(this.APIUrl+'/User/Login', account)
  }

  logout():Observable<any[]>{
    return this.http.post<any>(this.APIUrl+'/User/Logout', "")
  }

  register(account:any):Observable<any[]>{
    return this.http.post<any>(this.APIUrl+'/User/Register', account)
  }

  editUser(account:any):Observable<any[]>{
    return this.http.post<any>(this.APIUrl+'/User/Edit', account)
  }

  removeUser(id:any):Observable<any[]>{
    return this.http.post<any>(this.APIUrl+'/User/Remove', id)
  }

  recoverPassword(account:any):Observable<any[]>{
    return this.http.post<any>(this.APIUrl+'/User/RecoverPassword', account)
  }

  //#endregion

  //#region Movies

  movieDetails(id:any):Observable<any[]>{
    return this.http.get<any>(this.APIUrl+'/Movies/Details?id=' + id.toString())
  }

  movieCreate(movie:any):Observable<any[]>{
    return this.http.post<any>(this.APIUrl+'/Movies/Create', movie)
  }

  movieEdit(movie:any):Observable<any[]>{
    return this.http.post<any>(this.APIUrl+'/Movies/Edit', movie)
  }

  movieDelete(movie:any):Observable<any[]>{
    return this.http.post<any>(this.APIUrl+'/Movies/Delete', movie)
  }

  paginateMovies(model:any):Observable<any[]>{
    return this.http.post<any>(this.APIUrl+'/Movies/Paginate', model)
  }

  //#endregion

  //#region Genres

  listGenres():Observable<any>{
    return this.http.get<any>(this.APIUrl+'/Genre/List')
  }

  usedGenres():Observable<any>{
    return this.http.get<any>(this.APIUrl+'/Genre/UsedGenres')
  }

  createGenre(genre:any):Observable<any>{
    return this.http.post<any>(this.APIUrl+'/Genre/Create', genre)
  }

  deleteGenre(id:any){
    return this.http.delete(this.APIUrl+'/Genre/'+id);
  }

  paginateGenres(model:any):Observable<any>{
    return this.http.post<any>(this.APIUrl+'/Genre/Paginate', model)
  }

  //#endregion

  //#region Favourites

  updateFavourite(idWithToken:any):Observable<any>{
    return this.http.post<any>(this.APIUrl+'/Favourites/Update', idWithToken)
  }

  removeFavourite(idWithToken:any):Observable<any>{
    return this.http.post<any>(this.APIUrl+'/Favourites/Remove', idWithToken)
  }

  deleteFavourite(idWithToken:any):Observable<any>{
    return this.http.post<any>(this.APIUrl+'/Favourites/Delete', idWithToken)
  }

  paginateFavourites(model:any, token:string):Observable<any>{
    return this.http.post<any>(this.APIUrl+'/Favourites/Paginate?token=' + token, model)
  }

  //#endregion
}
