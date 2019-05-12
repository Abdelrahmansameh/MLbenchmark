from model import *


def index_to_move(index):
    list_of_moves = [(-1,0), (-1,1), (0,0), (0,1), (1,0), (1,1)]
    return list_of_moves[index]

def run_game_with_ML(weights):
    # Start playing a game until the player is not grounded
    # and compute the score
    score = 0
    grounded = False
    while not grounded:
        # get features about the game: x, y, grounded, near_plat_x
        x, y, grounded, near_plat_x = get_features()
        # get a prediction based on the current input
        predictions = []
        predicted_move_index = np.argmax(np.array(forward_propagation(np.array([x, y, grounded, near_plat_x]).reshape(-1, n_x), weights)))
        predicted_move = index_to_move[predicted_move_index]

        # compute score
        if grounded:
            score -= 1000
        else:
            score += x
        # TODO: send the new move to the game and continue playing

    return score
