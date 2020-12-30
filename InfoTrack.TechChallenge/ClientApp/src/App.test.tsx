import * as React from 'react';
// import * as ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import renderer from 'react-test-renderer';
import { MemoryRouter } from 'react-router-dom';
import App from './App';

it('InfoTrack Seo Check App renders without crashing', () => {
  const storeFake = (state: any) => ({
    default: () => {},
    subscribe: () => {},
    dispatch: () => {},
    getState: () => ({ ...state }),
  });
  const store = storeFake({
    configuration: {
      searchEngines: [],
    },
  }) as any;
  const tree = renderer
    .create(
      <Provider store={store}>
        <MemoryRouter>
          <App />
        </MemoryRouter>
      </Provider>
    )
    .toJSON();
  expect(tree).toMatchSnapshot();
  //   ReactDOM.render(
  //     <Provider store={store}>
  //       <MemoryRouter>
  //         <App />
  //       </MemoryRouter>
  //     </Provider>,
  //     document.createElement('div')
  //   );
});
