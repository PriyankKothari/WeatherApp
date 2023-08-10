import { Weather } from "./components/weather/Weather";
import { Home } from "./components/Home";

const AppRoutes = [
  {
    index: true,
    element: <Home />
  },
  {
    path: '/weather',
    element: <Weather />
  }
];

export default AppRoutes;
