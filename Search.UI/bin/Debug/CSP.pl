
colour_countries(Colours,List):-
        setof(Country/_, X^neighbour(Country,X), Colours),
	retractall(steps(flow,_)),
	assert(steps(flow,[])),
        colours(Colours),
	retract(steps(flow,OldSteps)),
	append(OldSteps,[],List).
	

colours([]).

colours([Country/Colour|Tail]):-
        colours(Tail),
        member(Colour, [blue,red,green]),
%	string_concat(Country,'/',T),
%	string_concat(T,Colour,S),

	retract(steps(flow,OldSteps)),
	append(OldSteps,[Country/Colour],NewSteps),
	assert(steps(flow,NewSteps)),

%	member(New,Var2),	
%	write(New),nl,
        \+ (member(SomeCountry/Colour, Tail), checkNeighbour(Country, SomeCountry)).

checkNeighbour(Country, SomeCountry):-
        neighbour(Country, NeighboursList),
        member(SomeCountry, NeighboursList).


member(X, [X|_]).
member(X, [_|T]):-
        member(X, T).

