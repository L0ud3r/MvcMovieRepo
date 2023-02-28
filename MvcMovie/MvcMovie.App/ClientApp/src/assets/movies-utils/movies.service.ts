/* eslint-disable @typescript-eslint/adjacent-overload-signatures */
import { Injectable, PipeTransform } from '@angular/core';

import { BehaviorSubject, Observable, of, Subject } from 'rxjs';

import { MovieModel } from '../../app/models/movie-model';
import { MOVIES } from 'src/assets/movies-utils/movies';
import { DecimalPipe } from '@angular/common';
import { debounceTime, delay, switchMap, tap } from 'rxjs/operators';
import { SortColumn, SortDirection } from './sortable.directive';

interface SearchResult {
	movies: MovieModel[];
	total: number;
}

interface State {
	page: number;
	pageSize: number;
	searchTerm: string;
	sortColumn: SortColumn;
	sortDirection: SortDirection;
}

const compare = (v1: string | number | Date, v2: string | number | Date ) => (v1 < v2 ? -1 : v1 > v2 ? 1 : 0);

function sort(movies: MovieModel[], column: SortColumn, direction: string): MovieModel[] {
	if (direction === '' || column === '') {
		return movies;
	} else {
		return [...movies].sort((a, b) => {
			const res = compare(a[column], b[column]);
			return direction === 'asc' ? res : -res;
		});
	}
}

function matches(movie: MovieModel, term: string, pipe: PipeTransform) {
	return (
		movie.title.toLowerCase().includes(term.toLowerCase()) ||
		pipe.transform(movie.genreName).includes(term) ||
		pipe.transform(movie.releaseDate).includes(term)
	);
}

@Injectable({ providedIn: 'root' })
export class movieService {
	private _loading$ = new BehaviorSubject<boolean>(true);
	private _search$ = new Subject<void>();
	private _movies$ = new BehaviorSubject<MovieModel[]>([]);
	private _total$ = new BehaviorSubject<number>(0);

	private _state: State = {
		page: 1,
		pageSize: 4,
		searchTerm: '',
		sortColumn: '',
		sortDirection: '',
	};

	constructor(private pipe: DecimalPipe) {
		this._search$
			.pipe(
				tap(() => this._loading$.next(true)),
				debounceTime(200),
				switchMap(() => this._search()),
				delay(200),
				tap(() => this._loading$.next(false)),
			)
			.subscribe((result) => {
				this._movies$.next(result.movies);
				this._total$.next(result.total);
			});

		this._search$.next();
	}

	get movies$() {
		return this._movies$.asObservable();
	}
	get total$() {
		return this._total$.asObservable();
	}
	get loading$() {
		return this._loading$.asObservable();
	}
	get page() {
		return this._state.page;
	}
	get pageSize() {
		return this._state.pageSize;
	}
	get searchTerm() {
		return this._state.searchTerm;
	}

	set page(page: number) {
		this._set({ page });
	}
	set pageSize(pageSize: number) {
		this._set({ pageSize });
	}
	set searchTerm(searchTerm: string) {
		this._set({ searchTerm });
	}
	set sortColumn(sortColumn: SortColumn) {
		this._set({ sortColumn });
	}
	set sortDirection(sortDirection: SortDirection) {
		this._set({ sortDirection });
	}

	private _set(patch: Partial<State>) {
		Object.assign(this._state, patch);
		this._search$.next();
	}

	private _search(): Observable<SearchResult> {
		const { sortColumn, sortDirection, pageSize, page, searchTerm } = this._state;

		// 1. sort
		let movies = sort(MOVIES, sortColumn, sortDirection);

		// 2. filter
		movies = movies.filter((movie) => matches(movie, searchTerm, this.pipe));
		const total = movies.length;

		// 3. paginate
		movies = movies.slice((page - 1) * pageSize, (page - 1) * pageSize + pageSize);
		return of({ movies, total });
	}
}
