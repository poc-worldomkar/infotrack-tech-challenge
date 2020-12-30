import { Action, Reducer } from "redux";

// -----------------
// STATE - This defines the type of data maintained in the Redux store.

export interface SearchEngine {
  searchEngineName: string;
  searchEngineBaseUrlPath: string | null;
  resultXpathSelector: string;
  staticPages: boolean;
  parameterNameQuery: string;
  parameterNamePage: string | null;
  parameterNamePageSize: string | null;
  parameterNameRecordsSkip: string | null;
  dynamicPageSize: boolean;
  indexStartsAtOne: boolean;
}
export interface ConfigurationState {
  searchEngines: SearchEngine[];
}

// -----------------
// ACTIONS - These are serializable (hence replayable) descriptions of state transitions.
// They do not themselves have any side-effects; they just describe something that is going to happen.
// Use @typeName and isActionType for type detection that works even after serialization/deserialization.

export interface SetSearchEnginesAction {
  type: "SET_SEARCH_ENGINES";
  payload: SearchEngine[];
}
export interface SetSearchEngineOptions {
  type: "SET_SEARCH_ENGINE_OPTIONS";
  payload: SearchEngine;
}

// Declare a 'discriminated union' type. This guarantees that all references to 'type' properties contain one of the
// declared type strings (and not any other arbitrary string).
export type KnownAction = SetSearchEnginesAction | SetSearchEngineOptions;

// ----------------
// ACTION CREATORS - These are functions exposed to UI components that will trigger a state transition.
// They don't directly mutate state, but they can have external side-effects (such as loading data).

export const actionCreators = {
  setSearchEngines: (payload: SearchEngine[]) =>
    ({
      type: "SET_SEARCH_ENGINES",
      payload,
    } as SetSearchEnginesAction),
  setActiveSearchEngine: (payload: SearchEngine) =>
    ({
      type: "SET_SEARCH_ENGINE_OPTIONS",
      payload,
    } as SetSearchEngineOptions),
};

// ----------------
// REDUCER - For a given state and action, returns the new state. To support time travel, this must not mutate the old state.

export const reducer: Reducer<ConfigurationState> = (
  state: ConfigurationState | undefined,
  incomingAction: Action
): ConfigurationState => {
  let newState: ConfigurationState = { searchEngines: [] };
  if (state === undefined) {
    return newState;
  }

  const action = incomingAction as KnownAction;
  switch (action.type) {
    case "SET_SEARCH_ENGINES":
      newState = { searchEngines: action.payload };
      return newState;
    default:
      return state;
  }
};
